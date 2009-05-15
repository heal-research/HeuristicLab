/*
 * SVM.NET Library
 * Copyright (C) 2008 Matthew Johnson
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */


using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SVM
{
    //
    // Kernel evaluation
    //
    // the static method k_function is for doing single kernel evaluation
    // the constructor of Kernel prepares to calculate the l*l kernel matrix
    // the member function get_Q is for getting one column from the Q Matrix
    //
    internal abstract class QMatrix
    {
        public abstract float[] get_Q(int column, int len);
        public abstract float[] get_QD();
        public abstract void swap_index(int i, int j);
    }

    internal abstract class Kernel : QMatrix
    {
        private Node[][] _x;
        private double[] _x_square;

        // Parameter
        private KernelType kernel_type;
        private int degree;
        private double gamma;
        private double coef0;

        public override void swap_index(int i, int j)
        {
            do { Node[] _ = _x[i]; _x[i] = _x[j]; _x[j] = _; } while (false);
            if (_x_square != null) do { double _ = _x_square[i]; _x_square[i] = _x_square[j]; _x_square[j] = _; } while (false);
        }

        private static double powi(double baseValue, int times)
        {
            double tmp = baseValue, ret = 1.0;

            for (int t = times; t > 0; t /= 2)
            {
                if (t % 2 == 1) ret *= tmp;
                tmp = tmp * tmp;
            }
            return ret;
        }

        private static double tanh(double x)
        {
            double e = Math.Exp(x);
            return 1.0 - 2.0 / (e * e + 1);
        }

        public double kernel_function(int i, int j)
        {
            switch (kernel_type)
            {
                case KernelType.LINEAR:
                    return dot(_x[i], _x[j]);
                case KernelType.POLY:
                    return powi(gamma * dot(_x[i], _x[j]) + coef0, degree);
                case KernelType.RBF:
                    return Math.Exp(-gamma * (_x_square[i] + _x_square[j] - 2 * dot(_x[i], _x[j])));
                case KernelType.SIGMOID:
                    return tanh(gamma * dot(_x[i], _x[j]) + coef0);
                case KernelType.PRECOMPUTED:
                    return _x[i][(int)(_x[j][0].Value)].Value;
                default:
                    return 0;
            }
        }

        public Kernel(int l, Node[][] x_, Parameter param)
        {
            this.kernel_type = param.KernelType;
            this.degree = param.Degree;
            this.gamma = param.Gamma;
            this.coef0 = param.Coefficient0;

            _x = (Node[][])x_.Clone();

            if (kernel_type == KernelType.RBF)
            {
                _x_square = new double[l];
                for (int i = 0; i < l; i++)
                    _x_square[i] = dot(_x[i], _x[i]);
            }
            else _x_square = null;
        }

        public static double dot(Node[] x, Node[] y)
        {
            double sum = 0;
            int xlen = x.Length;
            int ylen = y.Length;
            int i = 0;
            int j = 0;
            while (i < xlen && j < ylen)
            {
                if (x[i].Index == y[j].Index)
                    sum += x[i++].Value * y[j++].Value;
                else
                {
                    if (x[i].Index > y[j].Index)
                        ++j;
                    else
                        ++i;
                }
            }
            return sum;
        }

        public static double k_function(Node[] x, Node[] y, Parameter param)
        {
            switch (param.KernelType)
            {
                case KernelType.LINEAR:
                    return dot(x, y);
                case KernelType.POLY:
                    return powi(param.Gamma * dot(x, y) + param.Coefficient0, param.Degree);
                case KernelType.RBF:
                    {
                        double sum = 0;
                        int xlen = x.Length;
                        int ylen = y.Length;
                        int i = 0;
                        int j = 0;
                        while (i < xlen && j < ylen)
                        {
                            if (x[i].Index == y[j].Index)
                            {
                                double d = x[i++].Value - y[j++].Value;
                                sum += d * d;
                            }
                            else if (x[i].Index > y[j].Index)
                            {
                                sum += y[j].Value * y[j].Value;
                                ++j;
                            }
                            else
                            {
                                sum += x[i].Value * x[i].Value;
                                ++i;
                            }
                        }

                        while (i < xlen)
                        {
                            sum += x[i].Value * x[i].Value;
                            ++i;
                        }

                        while (j < ylen)
                        {
                            sum += y[j].Value * y[j].Value;
                            ++j;
                        }

                        return Math.Exp(-param.Gamma * sum);
                    }
                case KernelType.SIGMOID:
                    return tanh(param.Gamma * dot(x, y) + param.Coefficient0);
                case KernelType.PRECOMPUTED:
                    return x[(int)(y[0].Value)].Value;
                default:
                    return 0;
            }
        }
    }

    // An SMO algorithm in Fan et al., JMLR 6(2005), p. 1889--1918
    // Solves:
    //
    //	min 0.5(\alpha^T Q \alpha) + p^T \alpha
    //
    //		y^T \alpha = \delta
    //		y_i = +1 or -1
    //		0 <= alpha_i <= Cp for y_i = 1
    //		0 <= alpha_i <= Cn for y_i = -1
    //
    // Given:
    //
    //	Q, p, y, Cp, Cn, and an initial feasible point \alpha
    //	l is the size of vectors and matrices
    //	eps is the stopping tolerance
    //
    // solution will be put in \alpha, objective value will be put in obj
    //
    internal class Solver
    {
        protected int active_size;
        protected short[] y;
        protected double[] G;		// gradient of objective function
        protected const byte LOWER_BOUND = 0;
        protected const byte UPPER_BOUND = 1;
        protected const byte FREE = 2;
        protected byte[] alpha_status;	// LOWER_BOUND, UPPER_BOUND, FREE
        protected double[] alpha;
        protected QMatrix Q;
        protected float[] QD;
        protected double eps;
        protected double Cp, Cn;
        protected double[] p;
        protected int[] active_set;
        protected double[] G_bar;		// gradient, if we treat free variables as 0
        protected int l;
        protected bool unshrinked;	// XXX

        protected const double INF = double.PositiveInfinity;

        protected double get_C(int i)
        {
            return (y[i] > 0) ? Cp : Cn;
        }
        protected void update_alpha_status(int i)
        {
            if (alpha[i] >= get_C(i))
                alpha_status[i] = UPPER_BOUND;
            else if (alpha[i] <= 0)
                alpha_status[i] = LOWER_BOUND;
            else alpha_status[i] = FREE;
        }
        protected bool is_upper_bound(int i) { return alpha_status[i] == UPPER_BOUND; }
        protected bool is_lower_bound(int i) { return alpha_status[i] == LOWER_BOUND; }
        protected bool is_free(int i) { return alpha_status[i] == FREE; }

        // java: information about solution except alpha,
        // because we cannot return multiple values otherwise...
        internal class SolutionInfo
        {
            public double obj;
            public double rho;
            public double upper_bound_p;
            public double upper_bound_n;
            public double r;	// for Solver_NU
        }

        protected void swap_index(int i, int j)
        {
            Q.swap_index(i, j);
            do { short _ = y[i]; y[i] = y[j]; y[j] = _; } while (false);
            do { double _ = G[i]; G[i] = G[j]; G[j] = _; } while (false);
            do { byte _ = alpha_status[i]; alpha_status[i] = alpha_status[j]; alpha_status[j] = _; } while (false);
            do { double _ = alpha[i]; alpha[i] = alpha[j]; alpha[j] = _; } while (false);
            do { double _ = p[i]; p[i] = p[j]; p[j] = _; } while (false);
            do { int _ = active_set[i]; active_set[i] = active_set[j]; active_set[j] = _; } while (false);
            do { double _ = G_bar[i]; G_bar[i] = G_bar[j]; G_bar[j] = _; } while (false);
        }

        protected void reconstruct_gradient()
        {
            // reconstruct inactive elements of G from G_bar and free variables

            if (active_size == l) return;

            int i;
            for (i = active_size; i < l; i++)
                G[i] = G_bar[i] + p[i];

            for (i = 0; i < active_size; i++)
                if (is_free(i))
                {
                    float[] Q_i = Q.get_Q(i, l);
                    double alpha_i = alpha[i];
                    for (int j = active_size; j < l; j++)
                        G[j] += alpha_i * Q_i[j];
                }
        }

        public virtual void Solve(int l, QMatrix Q, double[] p_, short[] y_,
               double[] alpha_, double Cp, double Cn, double eps, SolutionInfo si, bool shrinking)
        {
            this.l = l;
            this.Q = Q;
            QD = Q.get_QD();
            p = (double[])p_.Clone();
            y = (short[])y_.Clone();
            alpha = (double[])alpha_.Clone();
            this.Cp = Cp;
            this.Cn = Cn;
            this.eps = eps;
            this.unshrinked = false;

            // initialize alpha_status
            {
                alpha_status = new byte[l];
                for (int i = 0; i < l; i++)
                    update_alpha_status(i);
            }

            // initialize active set (for shrinking)
            {
                active_set = new int[l];
                for (int i = 0; i < l; i++)
                    active_set[i] = i;
                active_size = l;
            }

            // initialize gradient
            {
                G = new double[l];
                G_bar = new double[l];
                int i;
                for (i = 0; i < l; i++)
                {
                    G[i] = p[i];
                    G_bar[i] = 0;
                }
                for (i = 0; i < l; i++)
                    if (!is_lower_bound(i))
                    {
                        float[] Q_i = Q.get_Q(i, l);
                        double alpha_i = alpha[i];
                        int j;
                        for (j = 0; j < l; j++)
                            G[j] += alpha_i * Q_i[j];
                        if (is_upper_bound(i))
                            for (j = 0; j < l; j++)
                                G_bar[j] += get_C(i) * Q_i[j];
                    }
            }

            // optimization step

            int iter = 0;
            int counter = Math.Min(l, 1000) + 1;
            int[] working_set = new int[2];

            while (true)
            {
                // show progress and do shrinking

                if (--counter == 0)
                {
                    counter = Math.Min(l, 1000);
                    if (shrinking) do_shrinking();
                    Debug.Write(".");
                }

                if (select_working_set(working_set) != 0)
                {
                    // reconstruct the whole gradient
                    reconstruct_gradient();
                    // reset active set size and check
                    active_size = l;
                    Debug.Write("*");
                    if (select_working_set(working_set) != 0)
                        break;
                    else
                        counter = 1;	// do shrinking next iteration
                }

                int i = working_set[0];
                int j = working_set[1];

                ++iter;

                // update alpha[i] and alpha[j], handle bounds carefully

                float[] Q_i = Q.get_Q(i, active_size);
                float[] Q_j = Q.get_Q(j, active_size);

                double C_i = get_C(i);
                double C_j = get_C(j);

                double old_alpha_i = alpha[i];
                double old_alpha_j = alpha[j];

                if (y[i] != y[j])
                {
                    double quad_coef = Q_i[i] + Q_j[j] + 2 * Q_i[j];
                    if (quad_coef <= 0)
                        quad_coef = 1e-12;
                    double delta = (-G[i] - G[j]) / quad_coef;
                    double diff = alpha[i] - alpha[j];
                    alpha[i] += delta;
                    alpha[j] += delta;

                    if (diff > 0)
                    {
                        if (alpha[j] < 0)
                        {
                            alpha[j] = 0;
                            alpha[i] = diff;
                        }
                    }
                    else
                    {
                        if (alpha[i] < 0)
                        {
                            alpha[i] = 0;
                            alpha[j] = -diff;
                        }
                    }
                    if (diff > C_i - C_j)
                    {
                        if (alpha[i] > C_i)
                        {
                            alpha[i] = C_i;
                            alpha[j] = C_i - diff;
                        }
                    }
                    else
                    {
                        if (alpha[j] > C_j)
                        {
                            alpha[j] = C_j;
                            alpha[i] = C_j + diff;
                        }
                    }
                }
                else
                {
                    double quad_coef = Q_i[i] + Q_j[j] - 2 * Q_i[j];
                    if (quad_coef <= 0)
                        quad_coef = 1e-12;
                    double delta = (G[i] - G[j]) / quad_coef;
                    double sum = alpha[i] + alpha[j];
                    alpha[i] -= delta;
                    alpha[j] += delta;

                    if (sum > C_i)
                    {
                        if (alpha[i] > C_i)
                        {
                            alpha[i] = C_i;
                            alpha[j] = sum - C_i;
                        }
                    }
                    else
                    {
                        if (alpha[j] < 0)
                        {
                            alpha[j] = 0;
                            alpha[i] = sum;
                        }
                    }
                    if (sum > C_j)
                    {
                        if (alpha[j] > C_j)
                        {
                            alpha[j] = C_j;
                            alpha[i] = sum - C_j;
                        }
                    }
                    else
                    {
                        if (alpha[i] < 0)
                        {
                            alpha[i] = 0;
                            alpha[j] = sum;
                        }
                    }
                }

                // update G

                double delta_alpha_i = alpha[i] - old_alpha_i;
                double delta_alpha_j = alpha[j] - old_alpha_j;

                for (int k = 0; k < active_size; k++)
                {
                    G[k] += Q_i[k] * delta_alpha_i + Q_j[k] * delta_alpha_j;
                }

                // update alpha_status and G_bar

                {
                    bool ui = is_upper_bound(i);
                    bool uj = is_upper_bound(j);
                    update_alpha_status(i);
                    update_alpha_status(j);
                    int k;
                    if (ui != is_upper_bound(i))
                    {
                        Q_i = Q.get_Q(i, l);
                        if (ui)
                            for (k = 0; k < l; k++)
                                G_bar[k] -= C_i * Q_i[k];
                        else
                            for (k = 0; k < l; k++)
                                G_bar[k] += C_i * Q_i[k];
                    }

                    if (uj != is_upper_bound(j))
                    {
                        Q_j = Q.get_Q(j, l);
                        if (uj)
                            for (k = 0; k < l; k++)
                                G_bar[k] -= C_j * Q_j[k];
                        else
                            for (k = 0; k < l; k++)
                                G_bar[k] += C_j * Q_j[k];
                    }
                }

            }

            // calculate rho

            si.rho = calculate_rho();

            // calculate objective value
            {
                double v = 0;
                int i;
                for (i = 0; i < l; i++)
                    v += alpha[i] * (G[i] + p[i]);

                si.obj = v / 2;
            }

            // put back the solution
            {
                for (int i = 0; i < l; i++)
                    alpha_[active_set[i]] = alpha[i];
            }

            si.upper_bound_p = Cp;
            si.upper_bound_n = Cn;

            Debug.Write("\noptimization finished, #iter = " + iter + "\n");
        }

        // return 1 if already optimal, return 0 otherwise
        protected virtual int select_working_set(int[] working_set)
        {
            // return i,j such that
            // i: maximizes -y_i * grad(f)_i, i in I_up(\alpha)
            // j: mimimizes the decrease of obj value
            //    (if quadratic coefficeint <= 0, replace it with tau)
            //    -y_j*grad(f)_j < -y_i*grad(f)_i, j in I_low(\alpha)

            double Gmax = -INF;
            double Gmax2 = -INF;
            int Gmax_idx = -1;
            int Gmin_idx = -1;
            double obj_diff_min = INF;

            for (int t = 0; t < active_size; t++)
                if (y[t] == +1)
                {
                    if (!is_upper_bound(t))
                        if (-G[t] >= Gmax)
                        {
                            Gmax = -G[t];
                            Gmax_idx = t;
                        }
                }
                else
                {
                    if (!is_lower_bound(t))
                        if (G[t] >= Gmax)
                        {
                            Gmax = G[t];
                            Gmax_idx = t;
                        }
                }

            int i = Gmax_idx;
            float[] Q_i = null;
            if (i != -1) // null Q_i not accessed: Gmax=-INF if i=-1
                Q_i = Q.get_Q(i, active_size);

            for (int j = 0; j < active_size; j++)
            {
                if (y[j] == +1)
                {
                    if (!is_lower_bound(j))
                    {
                        double grad_diff = Gmax + G[j];
                        if (G[j] >= Gmax2)
                            Gmax2 = G[j];
                        if (grad_diff > 0)
                        {
                            double obj_diff;
                            double quad_coef = Q_i[i] + QD[j] - 2 * y[i] * Q_i[j];
                            if (quad_coef > 0)
                                obj_diff = -(grad_diff * grad_diff) / quad_coef;
                            else
                                obj_diff = -(grad_diff * grad_diff) / 1e-12;

                            if (obj_diff <= obj_diff_min)
                            {
                                Gmin_idx = j;
                                obj_diff_min = obj_diff;
                            }
                        }
                    }
                }
                else
                {
                    if (!is_upper_bound(j))
                    {
                        double grad_diff = Gmax - G[j];
                        if (-G[j] >= Gmax2)
                            Gmax2 = -G[j];
                        if (grad_diff > 0)
                        {
                            double obj_diff;
                            double quad_coef = Q_i[i] + QD[j] + 2 * y[i] * Q_i[j];
                            if (quad_coef > 0)
                                obj_diff = -(grad_diff * grad_diff) / quad_coef;
                            else
                                obj_diff = -(grad_diff * grad_diff) / 1e-12;

                            if (obj_diff <= obj_diff_min)
                            {
                                Gmin_idx = j;
                                obj_diff_min = obj_diff;
                            }
                        }
                    }
                }
            }

            if (Gmax + Gmax2 < eps)
                return 1;

            working_set[0] = Gmax_idx;
            working_set[1] = Gmin_idx;
            return 0;
        }

        private bool be_shrunken(int i, double Gmax1, double Gmax2)
        {
            if (is_upper_bound(i))
            {
                if (y[i] == +1)
                    return (-G[i] > Gmax1);
                else
                    return (-G[i] > Gmax2);
            }
            else if (is_lower_bound(i))
            {
                if (y[i] == +1)
                    return (G[i] > Gmax2);
                else
                    return (G[i] > Gmax1);
            }
            else
                return (false);
        }

        protected virtual void do_shrinking()
        {
            int i;
            double Gmax1 = -INF;		// max { -y_i * grad(f)_i | i in I_up(\alpha) }
            double Gmax2 = -INF;		// max { y_i * grad(f)_i | i in I_low(\alpha) }

            // find maximal violating pair first
            for (i = 0; i < active_size; i++)
            {
                if (y[i] == +1)
                {
                    if (!is_upper_bound(i))
                    {
                        if (-G[i] >= Gmax1)
                            Gmax1 = -G[i];
                    }
                    if (!is_lower_bound(i))
                    {
                        if (G[i] >= Gmax2)
                            Gmax2 = G[i];
                    }
                }
                else
                {
                    if (!is_upper_bound(i))
                    {
                        if (-G[i] >= Gmax2)
                            Gmax2 = -G[i];
                    }
                    if (!is_lower_bound(i))
                    {
                        if (G[i] >= Gmax1)
                            Gmax1 = G[i];
                    }
                }
            }

            // shrink

            for (i = 0; i < active_size; i++)
                if (be_shrunken(i, Gmax1, Gmax2))
                {
                    active_size--;
                    while (active_size > i)
                    {
                        if (!be_shrunken(active_size, Gmax1, Gmax2))
                        {
                            swap_index(i, active_size);
                            break;
                        }
                        active_size--;
                    }
                }

            // unshrink, check all variables again before sealed iterations

            if (unshrinked || Gmax1 + Gmax2 > eps * 10) return;

            unshrinked = true;
            reconstruct_gradient();

            for (i = l - 1; i >= active_size; i--)
                if (!be_shrunken(i, Gmax1, Gmax2))
                {
                    while (active_size < i)
                    {
                        if (be_shrunken(active_size, Gmax1, Gmax2))
                        {
                            swap_index(i, active_size);
                            break;
                        }
                        active_size++;
                    }
                    active_size++;
                }
        }

        protected virtual double calculate_rho()
        {
            double r;
            int nr_free = 0;
            double ub = INF, lb = -INF, sum_free = 0;
            for (int i = 0; i < active_size; i++)
            {
                double yG = y[i] * G[i];

                if (is_lower_bound(i))
                {
                    if (y[i] > 0)
                        ub = Math.Min(ub, yG);
                    else
                        lb = Math.Max(lb, yG);
                }
                else if (is_upper_bound(i))
                {
                    if (y[i] < 0)
                        ub = Math.Min(ub, yG);
                    else
                        lb = Math.Max(lb, yG);
                }
                else
                {
                    ++nr_free;
                    sum_free += yG;
                }
            }

            if (nr_free > 0)
                r = sum_free / nr_free;
            else
                r = (ub + lb) / 2;

            return r;
        }

    }

    //
    // Solver for nu-svm classification and regression
    //
    // additional constraint: e^T \alpha = constant
    //
    sealed class Solver_NU : Solver
    {
        private SolutionInfo si;

        public override void Solve(int l, QMatrix Q, double[] p, short[] y,
               double[] alpha, double Cp, double Cn, double eps,
               SolutionInfo si, bool shrinking)
        {
            this.si = si;
            base.Solve(l, Q, p, y, alpha, Cp, Cn, eps, si, shrinking);
        }

        // return 1 if already optimal, return 0 otherwise
        protected override int select_working_set(int[] working_set)
        {
            // return i,j such that y_i = y_j and
            // i: maximizes -y_i * grad(f)_i, i in I_up(\alpha)
            // j: minimizes the decrease of obj value
            //    (if quadratic coefficeint <= 0, replace it with tau)
            //    -y_j*grad(f)_j < -y_i*grad(f)_i, j in I_low(\alpha)

            double Gmaxp = -INF;
            double Gmaxp2 = -INF;
            int Gmaxp_idx = -1;

            double Gmaxn = -INF;
            double Gmaxn2 = -INF;
            int Gmaxn_idx = -1;

            int Gmin_idx = -1;
            double obj_diff_min = INF;

            for (int t = 0; t < active_size; t++)
                if (y[t] == +1)
                {
                    if (!is_upper_bound(t))
                        if (-G[t] >= Gmaxp)
                        {
                            Gmaxp = -G[t];
                            Gmaxp_idx = t;
                        }
                }
                else
                {
                    if (!is_lower_bound(t))
                        if (G[t] >= Gmaxn)
                        {
                            Gmaxn = G[t];
                            Gmaxn_idx = t;
                        }
                }

            int ip = Gmaxp_idx;
            int iN = Gmaxn_idx;
            float[] Q_ip = null;
            float[] Q_in = null;
            if (ip != -1) // null Q_ip not accessed: Gmaxp=-INF if ip=-1
                Q_ip = Q.get_Q(ip, active_size);
            if (iN != -1)
                Q_in = Q.get_Q(iN, active_size);

            for (int j = 0; j < active_size; j++)
            {
                if (y[j] == +1)
                {
                    if (!is_lower_bound(j))
                    {
                        double grad_diff = Gmaxp + G[j];
                        if (G[j] >= Gmaxp2)
                            Gmaxp2 = G[j];
                        if (grad_diff > 0)
                        {
                            double obj_diff;
                            double quad_coef = Q_ip[ip] + QD[j] - 2 * Q_ip[j];
                            if (quad_coef > 0)
                                obj_diff = -(grad_diff * grad_diff) / quad_coef;
                            else
                                obj_diff = -(grad_diff * grad_diff) / 1e-12;

                            if (obj_diff <= obj_diff_min)
                            {
                                Gmin_idx = j;
                                obj_diff_min = obj_diff;
                            }
                        }
                    }
                }
                else
                {
                    if (!is_upper_bound(j))
                    {
                        double grad_diff = Gmaxn - G[j];
                        if (-G[j] >= Gmaxn2)
                            Gmaxn2 = -G[j];
                        if (grad_diff > 0)
                        {
                            double obj_diff;
                            double quad_coef = Q_in[iN] + QD[j] - 2 * Q_in[j];
                            if (quad_coef > 0)
                                obj_diff = -(grad_diff * grad_diff) / quad_coef;
                            else
                                obj_diff = -(grad_diff * grad_diff) / 1e-12;

                            if (obj_diff <= obj_diff_min)
                            {
                                Gmin_idx = j;
                                obj_diff_min = obj_diff;
                            }
                        }
                    }
                }
            }

            if (Math.Max(Gmaxp + Gmaxp2, Gmaxn + Gmaxn2) < eps)
                return 1;

            if (y[Gmin_idx] == +1)
                working_set[0] = Gmaxp_idx;
            else
                working_set[0] = Gmaxn_idx;
            working_set[1] = Gmin_idx;

            return 0;
        }

        private bool be_shrunken(int i, double Gmax1, double Gmax2, double Gmax3, double Gmax4)
        {
            if (is_upper_bound(i))
            {
                if (y[i] == +1)
                    return (-G[i] > Gmax1);
                else
                    return (-G[i] > Gmax4);
            }
            else if (is_lower_bound(i))
            {
                if (y[i] == +1)
                    return (G[i] > Gmax2);
                else
                    return (G[i] > Gmax3);
            }
            else
                return (false);
        }

        protected override void do_shrinking()
        {
            double Gmax1 = -INF;	// max { -y_i * grad(f)_i | y_i = +1, i in I_up(\alpha) }
            double Gmax2 = -INF;	// max { y_i * grad(f)_i | y_i = +1, i in I_low(\alpha) }
            double Gmax3 = -INF;	// max { -y_i * grad(f)_i | y_i = -1, i in I_up(\alpha) }
            double Gmax4 = -INF;	// max { y_i * grad(f)_i | y_i = -1, i in I_low(\alpha) }

            // find maximal violating pair first
            int i;
            for (i = 0; i < active_size; i++)
            {
                if (!is_upper_bound(i))
                {
                    if (y[i] == +1)
                    {
                        if (-G[i] > Gmax1) Gmax1 = -G[i];
                    }
                    else if (-G[i] > Gmax4) Gmax4 = -G[i];
                }
                if (!is_lower_bound(i))
                {
                    if (y[i] == +1)
                    {
                        if (G[i] > Gmax2) Gmax2 = G[i];
                    }
                    else if (G[i] > Gmax3) Gmax3 = G[i];
                }
            }

            // shrinking

            for (i = 0; i < active_size; i++)
                if (be_shrunken(i, Gmax1, Gmax2, Gmax3, Gmax4))
                {
                    active_size--;
                    while (active_size > i)
                    {
                        if (!be_shrunken(active_size, Gmax1, Gmax2, Gmax3, Gmax4))
                        {
                            swap_index(i, active_size);
                            break;
                        }
                        active_size--;
                    }
                }

            if (unshrinked || Math.Max(Gmax1 + Gmax2, Gmax3 + Gmax4) > eps * 10) return;

            unshrinked = true;
            reconstruct_gradient();

            for (i = l - 1; i >= active_size; i--)
                if (!be_shrunken(i, Gmax1, Gmax2, Gmax3, Gmax4))
                {
                    while (active_size < i)
                    {
                        if (be_shrunken(active_size, Gmax1, Gmax2, Gmax3, Gmax4))
                        {
                            swap_index(i, active_size);
                            break;
                        }
                        active_size++;
                    }
                    active_size++;
                }
        }

        protected override double calculate_rho()
        {
            int nr_free1 = 0, nr_free2 = 0;
            double ub1 = INF, ub2 = INF;
            double lb1 = -INF, lb2 = -INF;
            double sum_free1 = 0, sum_free2 = 0;

            for (int i = 0; i < active_size; i++)
            {
                if (y[i] == +1)
                {
                    if (is_lower_bound(i))
                        ub1 = Math.Min(ub1, G[i]);
                    else if (is_upper_bound(i))
                        lb1 = Math.Max(lb1, G[i]);
                    else
                    {
                        ++nr_free1;
                        sum_free1 += G[i];
                    }
                }
                else
                {
                    if (is_lower_bound(i))
                        ub2 = Math.Min(ub2, G[i]);
                    else if (is_upper_bound(i))
                        lb2 = Math.Max(lb2, G[i]);
                    else
                    {
                        ++nr_free2;
                        sum_free2 += G[i];
                    }
                }
            }

            double r1, r2;
            if (nr_free1 > 0)
                r1 = sum_free1 / nr_free1;
            else
                r1 = (ub1 + lb1) / 2;

            if (nr_free2 > 0)
                r2 = sum_free2 / nr_free2;
            else
                r2 = (ub2 + lb2) / 2;

            si.r = (r1 + r2) / 2;
            return (r1 - r2) / 2;
        }
    }

    //
    // Q matrices for various formulations
    //
    class SVC_Q : Kernel
    {
        private short[] y;
        private Cache cache;
        private float[] QD;

        public SVC_Q(Problem prob, Parameter param, short[] y_) : base(prob.Count, prob.X, param)
        {
            y = (short[])y_.Clone();
            cache = new Cache(prob.Count, (long)(param.CacheSize * (1 << 20)));
            QD = new float[prob.Count];
            for (int i = 0; i < prob.Count; i++)
                QD[i] = (float)kernel_function(i, i);
        }

        public override float[] get_Q(int i, int len)
        {
            float[][] data = new float[1][];
            int start;
            if ((start = cache.get_data(i, data, len)) < len)
            {
                for (int j = start; j < len; j++)
                    data[0][j] = (float)(y[i] * y[j] * kernel_function(i, j));
            }
            return data[0];
        }

        public override float[] get_QD()
        {
            return QD;
        }

        public override void swap_index(int i, int j)
        {
            cache.swap_index(i, j);
            base.swap_index(i, j);
            do { short _ = y[i]; y[i] = y[j]; y[j] = _; } while (false);
            do { float _ = QD[i]; QD[i] = QD[j]; QD[j] = _; } while (false);
        }
    }

    class ONE_CLASS_Q : Kernel
    {
        private Cache cache;
        private float[] QD;

        public ONE_CLASS_Q(Problem prob, Parameter param) : base(prob.Count, prob.X, param)
        {
            cache = new Cache(prob.Count, (long)(param.CacheSize * (1 << 20)));
            QD = new float[prob.Count];
            for (int i = 0; i < prob.Count; i++)
                QD[i] = (float)kernel_function(i, i);
        }

        public override float[] get_Q(int i, int len)
        {
            float[][] data = new float[1][];
            int start;
            if ((start = cache.get_data(i, data, len)) < len)
            {
                for (int j = start; j < len; j++)
                    data[0][j] = (float)kernel_function(i, j);
            }
            return data[0];
        }

        public override float[] get_QD()
        {
            return QD;
        }

        public override void swap_index(int i, int j)
        {
            cache.swap_index(i, j);
            base.swap_index(i, j);
            do { float _ = QD[i]; QD[i] = QD[j]; QD[j] = _; } while (false);
        }
    }

    class SVR_Q : Kernel
    {
        private int l;
        private Cache cache;
        private short[] sign;
        private int[] index;
        private int next_buffer;
        private float[][] buffer;
        private float[] QD;

        public SVR_Q(Problem prob, Parameter param)
            : base(prob.Count, prob.X, param)
        {
            l = prob.Count;
            cache = new Cache(l, (long)(param.CacheSize * (1 << 20)));
            QD = new float[2 * l];
            sign = new short[2 * l];
            index = new int[2 * l];
            for (int k = 0; k < l; k++)
            {
                sign[k] = 1;
                sign[k + l] = -1;
                index[k] = k;
                index[k + l] = k;
                QD[k] = (float)kernel_function(k, k);
                QD[k + l] = QD[k];
            }
            buffer = new float[2][];
            buffer[0] = new float[2 * l];
            buffer[1] = new float[2 * l];
            next_buffer = 0;
        }

        public override void swap_index(int i, int j)
        {
            do { short _ = sign[i]; sign[i] = sign[j]; sign[j] = _; } while (false);
            do { int _ = index[i]; index[i] = index[j]; index[j] = _; } while (false);
            do { float _ = QD[i]; QD[i] = QD[j]; QD[j] = _; } while (false);
        }

        public override float[] get_Q(int i, int len)
        {
            float[][] data = new float[1][];
            int real_i = index[i];
            if (cache.get_data(real_i, data, l) < l)
            {
                for (int j = 0; j < l; j++)
                    data[0][j] = (float)kernel_function(real_i, j);
            }

            // reorder and copy
            float[] buf = buffer[next_buffer];
            next_buffer = 1 - next_buffer;
            short si = sign[i];
            for (int j = 0; j < len; j++)
                buf[j] = si * sign[j] * data[0][index[j]];
            return buf;
        }

        public override float[] get_QD()
        {
            return QD;
        }
    }

    internal static class Procedures
    {
        //
        // construct and solve various formulations
        //
        private static void solve_c_svc(Problem prob, Parameter param,
                        double[] alpha, Solver.SolutionInfo si,
                        double Cp, double Cn)
        {
            int l = prob.Count;
            double[] minus_ones = new double[l];
            short[] y = new short[l];

            int i;

            for (i = 0; i < l; i++)
            {
                alpha[i] = 0;
                minus_ones[i] = -1;
                if (prob.Y[i] > 0) y[i] = +1; else y[i] = -1;
            }

            Solver s = new Solver();
            s.Solve(l, new SVC_Q(prob, param, y), minus_ones, y,
                alpha, Cp, Cn, param.EPS, si, param.Shrinking);

            double sum_alpha = 0;
            for (i = 0; i < l; i++)
                sum_alpha += alpha[i];

            if (Cp == Cn)
                Debug.Write("nu = " + sum_alpha / (Cp * prob.Count) + "\n");

            for (i = 0; i < l; i++)
                alpha[i] *= y[i];
        }

        private static void solve_nu_svc(Problem prob, Parameter param,
                        double[] alpha, Solver.SolutionInfo si)
        {
            int i;
            int l = prob.Count;
            double nu = param.Nu;

            short[] y = new short[l];

            for (i = 0; i < l; i++)
                if (prob.Y[i] > 0)
                    y[i] = +1;
                else
                    y[i] = -1;

            double sum_pos = nu * l / 2;
            double sum_neg = nu * l / 2;

            for (i = 0; i < l; i++)
                if (y[i] == +1)
                {
                    alpha[i] = Math.Min(1.0, sum_pos);
                    sum_pos -= alpha[i];
                }
                else
                {
                    alpha[i] = Math.Min(1.0, sum_neg);
                    sum_neg -= alpha[i];
                }

            double[] zeros = new double[l];

            for (i = 0; i < l; i++)
                zeros[i] = 0;

            Solver_NU s = new Solver_NU();
            s.Solve(l, new SVC_Q(prob, param, y), zeros, y,
                alpha, 1.0, 1.0, param.EPS, si, param.Shrinking);
            double r = si.r;

            Debug.Write("C = " + 1 / r + "\n");

            for (i = 0; i < l; i++)
                alpha[i] *= y[i] / r;

            si.rho /= r;
            si.obj /= (r * r);
            si.upper_bound_p = 1 / r;
            si.upper_bound_n = 1 / r;
        }

        private static void solve_one_class(Problem prob, Parameter param,
                            double[] alpha, Solver.SolutionInfo si)
        {
            int l = prob.Count;
            double[] zeros = new double[l];
            short[] ones = new short[l];
            int i;

            int n = (int)(param.Nu * prob.Count);	// # of alpha's at upper bound

            for (i = 0; i < n; i++)
                alpha[i] = 1;
            if (n < prob.Count)
                alpha[n] = param.Nu * prob.Count - n;
            for (i = n + 1; i < l; i++)
                alpha[i] = 0;

            for (i = 0; i < l; i++)
            {
                zeros[i] = 0;
                ones[i] = 1;
            }

            Solver s = new Solver();
            s.Solve(l, new ONE_CLASS_Q(prob, param), zeros, ones,
                alpha, 1.0, 1.0, param.EPS, si, param.Shrinking);
        }

        private static void solve_epsilon_svr(Problem prob, Parameter param,
                        double[] alpha, Solver.SolutionInfo si)
        {
            int l = prob.Count;
            double[] alpha2 = new double[2 * l];
            double[] linear_term = new double[2 * l];
            short[] y = new short[2 * l];
            int i;

            for (i = 0; i < l; i++)
            {
                alpha2[i] = 0;
                linear_term[i] = param.P - prob.Y[i];
                y[i] = 1;

                alpha2[i + l] = 0;
                linear_term[i + l] = param.P + prob.Y[i];
                y[i + l] = -1;
            }

            Solver s = new Solver();
            s.Solve(2 * l, new SVR_Q(prob, param), linear_term, y,
                alpha2, param.C, param.C, param.EPS, si, param.Shrinking);

            double sum_alpha = 0;
            for (i = 0; i < l; i++)
            {
                alpha[i] = alpha2[i] - alpha2[i + l];
                sum_alpha += Math.Abs(alpha[i]);
            }
            Debug.Write("nu = " + sum_alpha / (param.C * l) + "\n");
        }

        private static void solve_nu_svr(Problem prob, Parameter param,
                        double[] alpha, Solver.SolutionInfo si)
        {
            int l = prob.Count;
            double C = param.C;
            double[] alpha2 = new double[2 * l];
            double[] linear_term = new double[2 * l];
            short[] y = new short[2 * l];
            int i;

            double sum = C * param.Nu * l / 2;
            for (i = 0; i < l; i++)
            {
                alpha2[i] = alpha2[i + l] = Math.Min(sum, C);
                sum -= alpha2[i];

                linear_term[i] = -prob.Y[i];
                y[i] = 1;

                linear_term[i + l] = prob.Y[i];
                y[i + l] = -1;
            }

            Solver_NU s = new Solver_NU();
            s.Solve(2 * l, new SVR_Q(prob, param), linear_term, y, alpha2, C, C, param.EPS, si, param.Shrinking);

            Debug.Write("epsilon = " + (-si.r) + "\n");

            for (i = 0; i < l; i++)
                alpha[i] = alpha2[i] - alpha2[i + l];
        }

        //
        // decision_function
        //
        private class decision_function
        {
            public double[] alpha;
            public double rho;
        };

        static decision_function svm_train_one(
            Problem prob, Parameter param,
            double Cp, double Cn)
        {
            double[] alpha = new double[prob.Count];
            Solver.SolutionInfo si = new Solver.SolutionInfo();
            switch (param.SvmType)
            {
                case SvmType.C_SVC:
                    solve_c_svc(prob, param, alpha, si, Cp, Cn);
                    break;
                case SvmType.NU_SVC:
                    solve_nu_svc(prob, param, alpha, si);
                    break;
                case SvmType.ONE_CLASS:
                    solve_one_class(prob, param, alpha, si);
                    break;
                case SvmType.EPSILON_SVR:
                    solve_epsilon_svr(prob, param, alpha, si);
                    break;
                case SvmType.NU_SVR:
                    solve_nu_svr(prob, param, alpha, si);
                    break;
            }

            Debug.Write("obj = " + si.obj + ", rho = " + si.rho + "\n");

            // output SVs

            int nSV = 0;
            int nBSV = 0;
            for (int i = 0; i < prob.Count; i++)
            {
                if (Math.Abs(alpha[i]) > 0)
                {
                    ++nSV;
                    if (prob.Y[i] > 0)
                    {
                        if (Math.Abs(alpha[i]) >= si.upper_bound_p)
                            ++nBSV;
                    }
                    else
                    {
                        if (Math.Abs(alpha[i]) >= si.upper_bound_n)
                            ++nBSV;
                    }
                }
            }

            Debug.Write("nSV = " + nSV + ", nBSV = " + nBSV + "\n");

            decision_function f = new decision_function();
            f.alpha = alpha;
            f.rho = si.rho;
            return f;
        }

        // Platt's binary SVM Probablistic Output: an improvement from Lin et al.
        private static void sigmoid_train(int l, double[] dec_values, double[] labels,
                      double[] probAB)
        {
            double A, B;
            double prior1 = 0, prior0 = 0;
            int i;

            for (i = 0; i < l; i++)
                if (labels[i] > 0) prior1 += 1;
                else prior0 += 1;

            int max_iter = 100; 	// Maximal number of iterations
            double min_step = 1e-10;	// Minimal step taken in line search
            double sigma = 1e-3;	// For numerically strict PD of Hessian
            double eps = 1e-5;
            double hiTarget = (prior1 + 1.0) / (prior1 + 2.0);
            double loTarget = 1 / (prior0 + 2.0);
            double[] t = new double[l];
            double fApB, p, q, h11, h22, h21, g1, g2, det, dA, dB, gd, stepsize;
            double newA, newB, newf, d1, d2;
            int iter;

            // Initial Point and Initial Fun Value
            A = 0.0; B = Math.Log((prior0 + 1.0) / (prior1 + 1.0));
            double fval = 0.0;

            for (i = 0; i < l; i++)
            {
                if (labels[i] > 0) t[i] = hiTarget;
                else t[i] = loTarget;
                fApB = dec_values[i] * A + B;
                if (fApB >= 0)
                    fval += t[i] * fApB + Math.Log(1 + Math.Exp(-fApB));
                else
                    fval += (t[i] - 1) * fApB + Math.Log(1 + Math.Exp(fApB));
            }
            for (iter = 0; iter < max_iter; iter++)
            {
                // Update Gradient and Hessian (use H' = H + sigma I)
                h11 = sigma; // numerically ensures strict PD
                h22 = sigma;
                h21 = 0.0; g1 = 0.0; g2 = 0.0;
                for (i = 0; i < l; i++)
                {
                    fApB = dec_values[i] * A + B;
                    if (fApB >= 0)
                    {
                        p = Math.Exp(-fApB) / (1.0 + Math.Exp(-fApB));
                        q = 1.0 / (1.0 + Math.Exp(-fApB));
                    }
                    else
                    {
                        p = 1.0 / (1.0 + Math.Exp(fApB));
                        q = Math.Exp(fApB) / (1.0 + Math.Exp(fApB));
                    }
                    d2 = p * q;
                    h11 += dec_values[i] * dec_values[i] * d2;
                    h22 += d2;
                    h21 += dec_values[i] * d2;
                    d1 = t[i] - p;
                    g1 += dec_values[i] * d1;
                    g2 += d1;
                }

                // Stopping Criteria
                if (Math.Abs(g1) < eps && Math.Abs(g2) < eps)
                    break;

                // Finding Newton direction: -inv(H') * g
                det = h11 * h22 - h21 * h21;
                dA = -(h22 * g1 - h21 * g2) / det;
                dB = -(-h21 * g1 + h11 * g2) / det;
                gd = g1 * dA + g2 * dB;


                stepsize = 1; 		// Line Search
                while (stepsize >= min_step)
                {
                    newA = A + stepsize * dA;
                    newB = B + stepsize * dB;

                    // New function value
                    newf = 0.0;
                    for (i = 0; i < l; i++)
                    {
                        fApB = dec_values[i] * newA + newB;
                        if (fApB >= 0)
                            newf += t[i] * fApB + Math.Log(1 + Math.Exp(-fApB));
                        else
                            newf += (t[i] - 1) * fApB + Math.Log(1 + Math.Exp(fApB));
                    }
                    // Check sufficient decrease
                    if (newf < fval + 0.0001 * stepsize * gd)
                    {
                        A = newA; B = newB; fval = newf;
                        break;
                    }
                    else
                        stepsize = stepsize / 2.0;
                }

                if (stepsize < min_step)
                {
                    Debug.Write("Line search fails in two-class probability estimates\n");
                    break;
                }
            }

            if (iter >= max_iter)
                Debug.Write("Reaching maximal iterations in two-class probability estimates\n");
            probAB[0] = A; probAB[1] = B;
        }

        private static double sigmoid_predict(double decision_value, double A, double B)
        {
            double fApB = decision_value * A + B;
            if (fApB >= 0)
                return Math.Exp(-fApB) / (1.0 + Math.Exp(-fApB));
            else
                return 1.0 / (1 + Math.Exp(fApB));
        }

        // Method 2 from the multiclass_prob paper by Wu, Lin, and Weng
        private static void multiclass_probability(int k, double[,] r, double[] p)
	{
		int t,j;
		int iter = 0, max_iter=Math.Max(100,k);
		double[,] Q=new double[k,k];
		double[] Qp= new double[k];
		double pQp, eps=0.005/k;
	
		for (t=0;t<k;t++)
		{
			p[t]=1.0/k;  // Valid if k = 1
			Q[t,t]=0;
			for (j=0;j<t;j++)
			{
				Q[t,t]+=r[j,t]*r[j,t];
				Q[t,j]=Q[j,t];
			}
			for (j=t+1;j<k;j++)
			{
				Q[t,t]+=r[j,t]*r[j,t];
				Q[t,j]=-r[j,t]*r[t,j];
			}
		}
		for (iter=0;iter<max_iter;iter++)
		{
			// stopping condition, recalculate QP,pQP for numerical accuracy
			pQp=0;
			for (t=0;t<k;t++)
			{
				Qp[t]=0;
				for (j=0;j<k;j++)
					Qp[t]+=Q[t,j]*p[j];
				pQp+=p[t]*Qp[t];
			}
			double max_error=0;
			for (t=0;t<k;t++)
			{
				double error=Math.Abs(Qp[t]-pQp);
				if (error>max_error)
					max_error=error;
			}
			if (max_error<eps) break;
		
			for (t=0;t<k;t++)
			{
				double diff=(-Qp[t]+pQp)/Q[t,t];
				p[t]+=diff;
				pQp=(pQp+diff*(diff*Q[t,t]+2*Qp[t]))/(1+diff)/(1+diff);
				for (j=0;j<k;j++)
				{
					Qp[j]=(Qp[j]+diff*Q[t,j])/(1+diff);
					p[j]/=(1+diff);
				}
			}
		}
		if (iter>=max_iter)
			Debug.Write("Exceeds max_iter in multiclass_prob\n");
	}

        // Cross-validation decision values for probability estimates
        private static void svm_binary_svc_probability(Problem prob, Parameter param, double Cp, double Cn, double[] probAB)
        {
            Random rand = new Random();
            int i;
            int nr_fold = 5;
            int[] perm = new int[prob.Count];
            double[] dec_values = new double[prob.Count];

            // random shuffle
            for (i = 0; i < prob.Count; i++) perm[i] = i;
            for (i = 0; i < prob.Count; i++)
            {
                int j = i + (int)(rand.NextDouble() * (prob.Count - i));
                do { int _ = perm[i]; perm[i] = perm[j]; perm[j] = _; } while (false);
            }
            for (i = 0; i < nr_fold; i++)
            {
                int begin = i * prob.Count / nr_fold;
                int end = (i + 1) * prob.Count / nr_fold;
                int j, k;
                Problem subprob = new Problem();

                subprob.Count = prob.Count - (end - begin);
                subprob.X = new Node[subprob.Count][];
                subprob.Y = new double[subprob.Count];

                k = 0;
                for (j = 0; j < begin; j++)
                {
                    subprob.X[k] = prob.X[perm[j]];
                    subprob.Y[k] = prob.Y[perm[j]];
                    ++k;
                }
                for (j = end; j < prob.Count; j++)
                {
                    subprob.X[k] = prob.X[perm[j]];
                    subprob.Y[k] = prob.Y[perm[j]];
                    ++k;
                }
                int p_count = 0, n_count = 0;
                for (j = 0; j < k; j++)
                    if (subprob.Y[j] > 0)
                        p_count++;
                    else
                        n_count++;

                if (p_count == 0 && n_count == 0)
                    for (j = begin; j < end; j++)
                        dec_values[perm[j]] = 0;
                else if (p_count > 0 && n_count == 0)
                    for (j = begin; j < end; j++)
                        dec_values[perm[j]] = 1;
                else if (p_count == 0 && n_count > 0)
                    for (j = begin; j < end; j++)
                        dec_values[perm[j]] = -1;
                else
                {
                    Parameter subparam = (Parameter)param.Clone();
                    subparam.Probability = false;
                    subparam.C = 1.0;
                    subparam.WeightCount = 2;
                    subparam.WeightLabels = new int[2];
                    subparam.Weights = new double[2];
                    subparam.WeightLabels[0] = +1;
                    subparam.WeightLabels[1] = -1;
                    subparam.Weights[0] = Cp;
                    subparam.Weights[1] = Cn;
                    Model submodel = svm_train(subprob, subparam);
                    for (j = begin; j < end; j++)
                    {
                        double[] dec_value = new double[1];
                        svm_predict_values(submodel, prob.X[perm[j]], dec_value);
                        dec_values[perm[j]] = dec_value[0];
                        // ensure +1 -1 order; reason not using CV subroutine
                        dec_values[perm[j]] *= submodel.ClassLabels[0];
                    }
                }
            }
            sigmoid_train(prob.Count, dec_values, prob.Y, probAB);
        }

        // Return parameter of a Laplace distribution 
        private static double svm_svr_probability(Problem prob, Parameter param)
        {
            int i;
            int nr_fold = 5;
            double[] ymv = new double[prob.Count];
            double mae = 0;

            Parameter newparam = (Parameter)param.Clone();
            newparam.Probability = false;
            svm_cross_validation(prob, newparam, nr_fold, ymv, null);
            for (i = 0; i < prob.Count; i++)
            {
                ymv[i] = prob.Y[i] - ymv[i];
                mae += Math.Abs(ymv[i]);
            }
            mae /= prob.Count;
            double std = Math.Sqrt(2 * mae * mae);
            int count = 0;
            mae = 0;
            for (i = 0; i < prob.Count; i++)
                if (Math.Abs(ymv[i]) > 5 * std)
                    count = count + 1;
                else
                    mae += Math.Abs(ymv[i]);
            mae /= (prob.Count - count);
            Debug.Write("Prob. model for test data: target value = predicted value + z,\nz: Laplace distribution e^(-|z|/sigma)/(2sigma),sigma=" + mae + "\n");
            return mae;
        }

        // label: label name, start: begin of each class, count: #data of classes, perm: indices to the original data
        // perm, length l, must be allocated before calling this subroutine
        private static void svm_group_classes(Problem prob, int[] nr_class_ret, int[][] label_ret, int[][] start_ret, int[][] count_ret, int[] perm)
        {
            int l = prob.Count;
            int max_nr_class = 16;
            int nr_class = 0;
            int[] label = new int[max_nr_class];
            int[] count = new int[max_nr_class];
            int[] data_label = new int[l];
            int i;

            for (i = 0; i < l; i++)
            {
                int this_label = (int)(prob.Y[i]);
                int j;
                for (j = 0; j < nr_class; j++)
                {
                    if (this_label == label[j])
                    {
                        ++count[j];
                        break;
                    }
                }
                data_label[i] = j;
                if (j == nr_class)
                {
                    if (nr_class == max_nr_class)
                    {
                        max_nr_class *= 2;
                        int[] new_data = new int[max_nr_class];
                        Array.Copy(label, 0, new_data, 0, label.Length);
                        label = new_data;
                        new_data = new int[max_nr_class];
                        Array.Copy(count, 0, new_data, 0, count.Length);
                        count = new_data;
                    }
                    label[nr_class] = this_label;
                    count[nr_class] = 1;
                    ++nr_class;
                }
            }

            int[] start = new int[nr_class];
            start[0] = 0;
            for (i = 1; i < nr_class; i++)
                start[i] = start[i - 1] + count[i - 1];
            for (i = 0; i < l; i++)
            {
                perm[start[data_label[i]]] = i;
                ++start[data_label[i]];
            }
            start[0] = 0;
            for (i = 1; i < nr_class; i++)
                start[i] = start[i - 1] + count[i - 1];

            nr_class_ret[0] = nr_class;
            label_ret[0] = label;
            start_ret[0] = start;
            count_ret[0] = count;
        }

        //
        // Interface functions
        //
        public static Model svm_train(Problem prob, Parameter param)
        {
            Model model = new Model();
            model.Parameter = param;

            if (param.SvmType == SvmType.ONE_CLASS ||
               param.SvmType == SvmType.EPSILON_SVR ||
               param.SvmType == SvmType.NU_SVR)
            {
                // regression or one-class-svm
                model.NumberOfClasses = 2;
                model.ClassLabels = null;
                model.NumberOfSVPerClass = null;
                model.PairwiseProbabilityA = null; model.PairwiseProbabilityB = null;
                model.SupportVectorCoefficients = new double[1][];

                if (param.Probability &&
                   (param.SvmType == SvmType.EPSILON_SVR ||
                    param.SvmType == SvmType.NU_SVR))
                {
                    model.PairwiseProbabilityA = new double[1];
                    model.PairwiseProbabilityA[0] = svm_svr_probability(prob, param);
                }

                decision_function f = svm_train_one(prob, param, 0, 0);
                model.Rho = new double[1];
                model.Rho[0] = f.rho;

                int nSV = 0;
                int i;
                for (i = 0; i < prob.Count; i++)
                    if (Math.Abs(f.alpha[i]) > 0) ++nSV;
                model.SupportVectorCount = nSV;
                model.SupportVectors = new Node[nSV][];
                model.SupportVectorCoefficients[0] = new double[nSV];
                int j = 0;
                for (i = 0; i < prob.Count; i++)
                    if (Math.Abs(f.alpha[i]) > 0)
                    {
                        model.SupportVectors[j] = prob.X[i];
                        model.SupportVectorCoefficients[0][j] = f.alpha[i];
                        ++j;
                    }
            }
            else
            {
                // classification
                int l = prob.Count;
                int[] tmp_nr_class = new int[1];
                int[][] tmp_label = new int[1][];
                int[][] tmp_start = new int[1][];
                int[][] tmp_count = new int[1][];
                int[] perm = new int[l];

                // group training data of the same class
                svm_group_classes(prob, tmp_nr_class, tmp_label, tmp_start, tmp_count, perm);
                int nr_class = tmp_nr_class[0];
                int[] label = tmp_label[0];
                int[] start = tmp_start[0];
                int[] count = tmp_count[0];
                Node[][] x = new Node[l][];
                int i;
                for (i = 0; i < l; i++)
                    x[i] = prob.X[perm[i]];

                // calculate weighted C

                double[] weighted_C = new double[nr_class];
                for (i = 0; i < nr_class; i++)
                    weighted_C[i] = param.C;
                for (i = 0; i < param.WeightCount; i++)
                {
                    int j;
                    for (j = 0; j < nr_class; j++)
                        if (param.WeightLabels[i] == label[j])
                            break;
                    if (j == nr_class)
                        Debug.Write("warning: class label " + param.WeightLabels[i] + " specified in weight is not found\n");
                    else
                        weighted_C[j] *= param.Weights[i];
                }

                // train k*(k-1)/2 models

                bool[] nonzero = new bool[l];
                for (i = 0; i < l; i++)
                    nonzero[i] = false;
                decision_function[] f = new decision_function[nr_class * (nr_class - 1) / 2];

                double[] probA = null, probB = null;
                if (param.Probability)
                {
                    probA = new double[nr_class * (nr_class - 1) / 2];
                    probB = new double[nr_class * (nr_class - 1) / 2];
                }

                int p = 0;
                for (i = 0; i < nr_class; i++)
                    for (int j = i + 1; j < nr_class; j++)
                    {
                        Problem sub_prob = new Problem();
                        int si = start[i], sj = start[j];
                        int ci = count[i], cj = count[j];
                        sub_prob.Count = ci + cj;
                        sub_prob.X = new Node[sub_prob.Count][];
                        sub_prob.Y = new double[sub_prob.Count];
                        int k;
                        for (k = 0; k < ci; k++)
                        {
                            sub_prob.X[k] = x[si + k];
                            sub_prob.Y[k] = +1;
                        }
                        for (k = 0; k < cj; k++)
                        {
                            sub_prob.X[ci + k] = x[sj + k];
                            sub_prob.Y[ci + k] = -1;
                        }

                        if (param.Probability)
                        {
                            double[] probAB = new double[2];
                            svm_binary_svc_probability(sub_prob, param, weighted_C[i], weighted_C[j], probAB);
                            probA[p] = probAB[0];
                            probB[p] = probAB[1];
                        }

                        f[p] = svm_train_one(sub_prob, param, weighted_C[i], weighted_C[j]);
                        for (k = 0; k < ci; k++)
                            if (!nonzero[si + k] && Math.Abs(f[p].alpha[k]) > 0)
                                nonzero[si + k] = true;
                        for (k = 0; k < cj; k++)
                            if (!nonzero[sj + k] && Math.Abs(f[p].alpha[ci + k]) > 0)
                                nonzero[sj + k] = true;
                        ++p;
                    }

                // build output

                model.NumberOfClasses = nr_class;

                model.ClassLabels = new int[nr_class];
                for (i = 0; i < nr_class; i++)
                    model.ClassLabels[i] = label[i];

                model.Rho = new double[nr_class * (nr_class - 1) / 2];
                for (i = 0; i < nr_class * (nr_class - 1) / 2; i++)
                    model.Rho[i] = f[i].rho;

                if (param.Probability)
                {
                    model.PairwiseProbabilityA = new double[nr_class * (nr_class - 1) / 2];
                    model.PairwiseProbabilityB = new double[nr_class * (nr_class - 1) / 2];
                    for (i = 0; i < nr_class * (nr_class - 1) / 2; i++)
                    {
                        model.PairwiseProbabilityA[i] = probA[i];
                        model.PairwiseProbabilityB[i] = probB[i];
                    }
                }
                else
                {
                    model.PairwiseProbabilityA = null;
                    model.PairwiseProbabilityB = null;
                }

                int nnz = 0;
                int[] nz_count = new int[nr_class];
                model.NumberOfSVPerClass = new int[nr_class];
                for (i = 0; i < nr_class; i++)
                {
                    int nSV = 0;
                    for (int j = 0; j < count[i]; j++)
                        if (nonzero[start[i] + j])
                        {
                            ++nSV;
                            ++nnz;
                        }
                    model.NumberOfSVPerClass[i] = nSV;
                    nz_count[i] = nSV;
                }

                Debug.Write("Total nSV = " + nnz + "\n");

                model.SupportVectorCount = nnz;
                model.SupportVectors = new Node[nnz][];
                p = 0;
                for (i = 0; i < l; i++)
                    if (nonzero[i]) model.SupportVectors[p++] = x[i];

                int[] nz_start = new int[nr_class];
                nz_start[0] = 0;
                for (i = 1; i < nr_class; i++)
                    nz_start[i] = nz_start[i - 1] + nz_count[i - 1];

                model.SupportVectorCoefficients = new double[nr_class - 1][];
                for (i = 0; i < nr_class - 1; i++)
                    model.SupportVectorCoefficients[i] = new double[nnz];

                p = 0;
                for (i = 0; i < nr_class; i++)
                    for (int j = i + 1; j < nr_class; j++)
                    {
                        // classifier (i,j): coefficients with
                        // i are in sv_coef[j-1][nz_start[i]...],
                        // j are in sv_coef[i][nz_start[j]...]

                        int si = start[i];
                        int sj = start[j];
                        int ci = count[i];
                        int cj = count[j];

                        int q = nz_start[i];
                        int k;
                        for (k = 0; k < ci; k++)
                            if (nonzero[si + k])
                                model.SupportVectorCoefficients[j - 1][q++] = f[p].alpha[k];
                        q = nz_start[j];
                        for (k = 0; k < cj; k++)
                            if (nonzero[sj + k])
                                model.SupportVectorCoefficients[i][q++] = f[p].alpha[ci + k];
                        ++p;
                    }
            }
            return model;
        }

        // Stratified cross validation
        public static void svm_cross_validation(Problem prob, Parameter param, int nr_fold, double[] target, Dictionary<int,double>[] confidence)
        {
            Random rand = new Random();
            int i;
            int[] fold_start = new int[nr_fold + 1];
            int l = prob.Count;
            int[] perm = new int[l];

            // stratified cv may not give leave-one-out rate
            // Each class to l folds -> some folds may have zero elements
            if ((param.SvmType == SvmType.C_SVC ||
                param.SvmType == SvmType.NU_SVC) && nr_fold < l)
            {
                int[] tmp_nr_class = new int[1];
                int[][] tmp_label = new int[1][];
                int[][] tmp_start = new int[1][];
                int[][] tmp_count = new int[1][];

                svm_group_classes(prob, tmp_nr_class, tmp_label, tmp_start, tmp_count, perm);

                int nr_class = tmp_nr_class[0];
                int[] label = tmp_label[0];
                int[] start = tmp_start[0];
                int[] count = tmp_count[0];

                // random shuffle and then data grouped by fold using the array perm
                int[] fold_count = new int[nr_fold];
                int c;
                int[] index = new int[l];
                for (i = 0; i < l; i++)
                    index[i] = perm[i];
                for (c = 0; c < nr_class; c++)
                    for (i = 0; i < count[c]; i++)
                    {
                        int j = i + (int)(rand.NextDouble() * (count[c] - i));
                        do { int _ = index[start[c] + j]; index[start[c] + j] = index[start[c] + i]; index[start[c] + i] = _; } while (false);
                    }
                for (i = 0; i < nr_fold; i++)
                {
                    fold_count[i] = 0;
                    for (c = 0; c < nr_class; c++)
                        fold_count[i] += (i + 1) * count[c] / nr_fold - i * count[c] / nr_fold;
                }
                fold_start[0] = 0;
                for (i = 1; i <= nr_fold; i++)
                    fold_start[i] = fold_start[i - 1] + fold_count[i - 1];
                for (c = 0; c < nr_class; c++)
                    for (i = 0; i < nr_fold; i++)
                    {
                        int begin = start[c] + i * count[c] / nr_fold;
                        int end = start[c] + (i + 1) * count[c] / nr_fold;
                        for (int j = begin; j < end; j++)
                        {
                            perm[fold_start[i]] = index[j];
                            fold_start[i]++;
                        }
                    }
                fold_start[0] = 0;
                for (i = 1; i <= nr_fold; i++)
                    fold_start[i] = fold_start[i - 1] + fold_count[i - 1];
            }
            else
            {
                for (i = 0; i < l; i++) perm[i] = i;
                for (i = 0; i < l; i++)
                {
                    int j = i + (int)(rand.NextDouble() * (l - i));
                    do { int _ = perm[i]; perm[i] = perm[j]; perm[j] = _; } while (false);
                }
                for (i = 0; i <= nr_fold; i++)
                    fold_start[i] = i * l / nr_fold;
            }

            for (i = 0; i < nr_fold; i++)
            {
                int begin = fold_start[i];
                int end = fold_start[i + 1];
                int j, k;
                Problem subprob = new Problem();

                subprob.Count = l - (end - begin);
                subprob.X = new Node[subprob.Count][];
                subprob.Y = new double[subprob.Count];

                k = 0;
                for (j = 0; j < begin; j++)
                {
                    subprob.X[k] = prob.X[perm[j]];
                    subprob.Y[k] = prob.Y[perm[j]];
                    ++k;
                }
                for (j = end; j < l; j++)
                {
                    subprob.X[k] = prob.X[perm[j]];
                    subprob.Y[k] = prob.Y[perm[j]];
                    ++k;
                }
                Model submodel = svm_train(subprob, param);
                if (param.Probability &&
                   (param.SvmType == SvmType.C_SVC ||
                    param.SvmType == SvmType.NU_SVC))
                {
                    for (j = begin; j < end; j++)
                    {
                        double[] prob_estimates = new double[svm_get_nr_class(submodel)];
                        target[perm[j]] = svm_predict_probability(submodel, prob.X[perm[j]], prob_estimates);
                        confidence[perm[j]] = new Dictionary<int, double>();
                        for (int label = 0; label < prob_estimates.Length; label++)
                            confidence[perm[j]][submodel.ClassLabels[label]] = prob_estimates[label];

                    }
                }
                else
                    for (j = begin; j < end; j++)
                        target[perm[j]] = svm_predict(submodel, prob.X[perm[j]]);
            }
        }

        public static SvmType svm_get_svm_type(Model model)
        {
            return model.Parameter.SvmType;
        }

        public static int svm_get_nr_class(Model model)
        {
            return model.NumberOfClasses;
        }

        public static void svm_get_labels(Model model, int[] label)
        {
            if (model.ClassLabels != null)
                for (int i = 0; i < model.NumberOfClasses; i++)
                    label[i] = model.ClassLabels[i];
        }

        public static double svm_get_svr_probability(Model model)
        {
            if ((model.Parameter.SvmType == SvmType.EPSILON_SVR || model.Parameter.SvmType == SvmType.NU_SVR) &&
                model.PairwiseProbabilityA != null)
                return model.PairwiseProbabilityA[0];
            else
            {
                Debug.Write("Model doesn't contain information for SVR probability inference\n");
                return 0;
            }
        }

        public static void svm_predict_values(Model model, Node[] x, double[] dec_values)
        {
            if (model.Parameter.SvmType == SvmType.ONE_CLASS ||
               model.Parameter.SvmType == SvmType.EPSILON_SVR ||
               model.Parameter.SvmType == SvmType.NU_SVR)
            {
                double[] sv_coef = model.SupportVectorCoefficients[0];
                double sum = 0;
                for (int i = 0; i < model.SupportVectorCount; i++)
                    sum += sv_coef[i] * Kernel.k_function(x, model.SupportVectors[i], model.Parameter);
                sum -= model.Rho[0];
                dec_values[0] = sum;
            }
            else
            {
                int i;
                int nr_class = model.NumberOfClasses;
                int l = model.SupportVectorCount;

                double[] kvalue = new double[l];
                for (i = 0; i < l; i++)
                    kvalue[i] = Kernel.k_function(x, model.SupportVectors[i], model.Parameter);

                int[] start = new int[nr_class];
                start[0] = 0;
                for (i = 1; i < nr_class; i++)
                    start[i] = start[i - 1] + model.NumberOfSVPerClass[i - 1];

                int p = 0;
                for (i = 0; i < nr_class; i++)
                    for (int j = i + 1; j < nr_class; j++)
                    {
                        double sum = 0;
                        int si = start[i];
                        int sj = start[j];
                        int ci = model.NumberOfSVPerClass[i];
                        int cj = model.NumberOfSVPerClass[j];

                        int k;
                        double[] coef1 = model.SupportVectorCoefficients[j - 1];
                        double[] coef2 = model.SupportVectorCoefficients[i];
                        for (k = 0; k < ci; k++)
                            sum += coef1[si + k] * kvalue[si + k];
                        for (k = 0; k < cj; k++)
                            sum += coef2[sj + k] * kvalue[sj + k];
                        sum -= model.Rho[p];
                        dec_values[p] = sum;
                        p++;
                    }
            }
        }

        public static double svm_predict(Model model, Node[] x)
        {
            if (model.Parameter.SvmType == SvmType.ONE_CLASS ||
               model.Parameter.SvmType == SvmType.EPSILON_SVR ||
               model.Parameter.SvmType == SvmType.NU_SVR)
            {
                double[] res = new double[1];
                svm_predict_values(model, x, res);

                if (model.Parameter.SvmType == SvmType.ONE_CLASS)
                    return (res[0] > 0) ? 1 : -1;
                else
                    return res[0];
            }
            else
            {
                int i;
                int nr_class = model.NumberOfClasses;
                double[] dec_values = new double[nr_class * (nr_class - 1) / 2];
                svm_predict_values(model, x, dec_values);

                int[] vote = new int[nr_class];
                for (i = 0; i < nr_class; i++)
                    vote[i] = 0;
                int pos = 0;
                for (i = 0; i < nr_class; i++)
                    for (int j = i + 1; j < nr_class; j++)
                    {
                        if (dec_values[pos++] > 0)
                            ++vote[i];
                        else
                            ++vote[j];
                    }

                int vote_max_idx = 0;
                for (i = 1; i < nr_class; i++)
                    if (vote[i] > vote[vote_max_idx])
                        vote_max_idx = i;
                return model.ClassLabels[vote_max_idx];
            }
        }

        public static double svm_predict_probability(Model model, Node[] x, double[] prob_estimates)
	{
        if ((model.Parameter.SvmType == SvmType.C_SVC || model.Parameter.SvmType == SvmType.NU_SVC) &&
		    model.PairwiseProbabilityA!=null && model.PairwiseProbabilityB!=null)
		{
			int i;
			int nr_class = model.NumberOfClasses;
			double[] dec_values = new double[nr_class*(nr_class-1)/2];
			svm_predict_values(model, x, dec_values);

			double min_prob=1e-7;
			double[,] pairwise_prob=new double[nr_class,nr_class];
			
			int k=0;
			for(i=0;i<nr_class;i++)
				for(int j=i+1;j<nr_class;j++)
				{
					pairwise_prob[i,j]=Math.Min(Math.Max(sigmoid_predict(dec_values[k],model.PairwiseProbabilityA[k],model.PairwiseProbabilityB[k]),min_prob),1-min_prob);
					pairwise_prob[j,i]=1-pairwise_prob[i,j];
					k++;
				}
			multiclass_probability(nr_class,pairwise_prob,prob_estimates);

			int prob_max_idx = 0;
			for(i=1;i<nr_class;i++)
				if(prob_estimates[i] > prob_estimates[prob_max_idx])
					prob_max_idx = i;
			return model.ClassLabels[prob_max_idx];
		}
		else 
			return svm_predict(model, x);
	}

        private static double atof(string s)
        {
            return double.Parse(s);
        }

        private static int atoi(string s)
        {
            return int.Parse(s);
        }

        public static string svm_check_parameter(Problem prob, Parameter param)
        {
            // svm_type

            SvmType svm_type = param.SvmType;
            if (svm_type != SvmType.C_SVC &&
               svm_type != SvmType.NU_SVC &&
               svm_type != SvmType.ONE_CLASS &&
               svm_type != SvmType.EPSILON_SVR &&
               svm_type != SvmType.NU_SVR)
                return "unknown svm type";

            // kernel_type, degree

            KernelType kernel_type = param.KernelType;
            if (kernel_type != KernelType.LINEAR &&
               kernel_type != KernelType.POLY &&
               kernel_type != KernelType.RBF &&
               kernel_type != KernelType.SIGMOID &&
               kernel_type != KernelType.PRECOMPUTED)
                return "unknown kernel type";

            if (param.Degree < 0)
                return "degree of polynomial kernel < 0";

            // cache_size,eps,C,nu,p,shrinking

            if (param.CacheSize <= 0)
                return "cache_size <= 0";

            if (param.EPS <= 0)
                return "eps <= 0";

            if (svm_type == SvmType.C_SVC ||
               svm_type == SvmType.EPSILON_SVR ||
               svm_type == SvmType.NU_SVR)
                if (param.C <= 0)
                    return "C <= 0";

            if (svm_type == SvmType.NU_SVC ||
               svm_type == SvmType.ONE_CLASS ||
               svm_type == SvmType.NU_SVR)
                if (param.Nu <= 0 || param.Nu > 1)
                    return "nu <= 0 or nu > 1";

            if (svm_type == SvmType.EPSILON_SVR)
                if (param.P < 0)
                    return "p < 0";

            if (param.Probability && svm_type == SvmType.ONE_CLASS)
                return "one-class SVM probability output not supported yet";

            // check whether nu-svc is feasible

            if (svm_type == SvmType.NU_SVC)
            {
                int l = prob.Count;
                int max_nr_class = 16;
                int nr_class = 0;
                int[] label = new int[max_nr_class];
                int[] count = new int[max_nr_class];

                int i;
                for (i = 0; i < l; i++)
                {
                    int this_label = (int)prob.Y[i];
                    int j;
                    for (j = 0; j < nr_class; j++)
                        if (this_label == label[j])
                        {
                            ++count[j];
                            break;
                        }

                    if (j == nr_class)
                    {
                        if (nr_class == max_nr_class)
                        {
                            max_nr_class *= 2;
                            int[] new_data = new int[max_nr_class];
                            Array.Copy(label, 0, new_data, 0, label.Length);
                            label = new_data;

                            new_data = new int[max_nr_class];
                            Array.Copy(count, 0, new_data, 0, count.Length);
                            count = new_data;
                        }
                        label[nr_class] = this_label;
                        count[nr_class] = 1;
                        ++nr_class;
                    }
                }

                for (i = 0; i < nr_class; i++)
                {
                    int n1 = count[i];
                    for (int j = i + 1; j < nr_class; j++)
                    {
                        int n2 = count[j];
                        if (param.Nu * (n1 + n2) / 2 > Math.Min(n1, n2))
                            return "specified nu is infeasible";
                    }
                }
            }

            return null;
        }

        public static int svm_check_probability_model(Model model)
        {
            if (((model.Parameter.SvmType == SvmType.C_SVC || model.Parameter.SvmType == SvmType.NU_SVC) &&
            model.PairwiseProbabilityA != null && model.PairwiseProbabilityB != null) ||
            ((model.Parameter.SvmType == SvmType.EPSILON_SVR || model.Parameter.SvmType == SvmType.NU_SVR) &&
             model.PairwiseProbabilityA != null))
                return 1;
            else
                return 0;
        }
    }

}
