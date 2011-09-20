#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  [Item("TimeSeriesPrognosisProblemData", "Represents an item containing all data defining a time series prognosis problem.")]
  public class TimeSeriesPrognosisProblemData : DataAnalysisProblemData, ITimeSeriesPrognosisProblemData {
    protected const string TargetVariableParameterName = "TargetVariable";

    #region default data
    private static double[,] mackey_glass_17 = new double[,] {
{9.0000000e-01},
{9.0000000e-01},
{9.0000000e-01},
{9.4136145e-01},
{1.1188041e+00},
{1.2161867e+00},
{1.2476827e+00},
{1.0067861e+00},
{7.1297054e-01},
{5.0349420e-01},
{5.8392542e-01},
{9.4327347e-01},
{1.0178784e+00},
{1.0346794e+00},
{1.1640420e+00},
{1.0670009e+00},
{1.0026658e+00},
{7.7000347e-01},
{7.0782984e-01},
{7.9370192e-01},
{1.0558440e+00},
{1.2000510e+00},
{1.2998549e+00},
{1.1857500e+00},
{8.4250325e-01},
{5.5885374e-01},
{4.2572016e-01},
{7.5043060e-01},
{9.7291951e-01},
{9.3785503e-01},
{1.0653587e+00},
{1.1355961e+00},
{1.1533842e+00},
{1.0678134e+00},
{8.1349649e-01},
{6.6246660e-01},
{6.2210481e-01},
{9.3890430e-01},
{1.1287058e+00},
{1.1699535e+00},
{1.2412759e+00},
{9.8090748e-01},
{7.3311051e-01},
{5.2596003e-01},
{6.3134850e-01},
{9.7849655e-01},
{1.0578901e+00},
{1.0888001e+00},
{1.1681297e+00},
{9.9896471e-01},
{8.7928027e-01},
{6.6864116e-01},
{7.3694626e-01},
{9.7850487e-01},
{1.1571565e+00},
{1.2440406e+00},
{1.2532851e+00},
{9.3967388e-01},
{6.5403177e-01},
{4.5549437e-01},
{6.4110939e-01},
{9.6727507e-01},
{9.8419122e-01},
{1.0332443e+00},
{1.1440570e+00},
{1.0922307e+00},
{1.0485820e+00},
{8.0447560e-01},
{7.1465526e-01},
{7.1296010e-01},
{9.9300973e-01},
{1.1766813e+00},
{1.2618288e+00},
{1.2517022e+00},
{9.2078170e-01},
{6.2369710e-01},
{4.3562308e-01},
{6.4988558e-01},
{9.5895791e-01},
{9.5866122e-01},
{1.0183775e+00},
{1.1415533e+00},
{1.1289579e+00},
{1.1022163e+00},
{8.4414928e-01},
{6.9891813e-01},
{6.2493643e-01},
{9.0049230e-01},
{1.1246569e+00},
{1.1850486e+00},
{1.2646400e+00},
{1.0193893e+00},
{7.3830722e-01},
{5.1921750e-01},
{5.6836076e-01},
{9.3609361e-01},
{1.0324202e+00},
{1.0360676e+00},
{1.1675966e+00},
{1.0504944e+00},
{9.8129158e-01},
{7.5798016e-01},
{7.1660079e-01},
{8.3950658e-01},
{1.0870027e+00},
{1.2170776e+00},
{1.3123779e+00},
{1.1361782e+00},
{7.9337369e-01},
{5.2230472e-01},
{4.3857925e-01},
{8.0873950e-01},
{9.7422631e-01},
{9.3291515e-01},
{1.0947530e+00},
{1.1335086e+00},
{1.1645714e+00},
{1.0315895e+00},
{7.9058261e-01},
{6.3878936e-01},
{6.4385427e-01},
{9.7290237e-01},
{1.1330941e+00},
{1.1738960e+00},
{1.2210669e+00},
{9.4813374e-01},
{7.1548595e-01},
{5.2046031e-01},
{6.8100333e-01},
{1.0072944e+00},
{1.0641199e+00},
{1.1194312e+00},
{1.1522817e+00},
{9.7118023e-01},
{8.2966445e-01},
{6.3876024e-01},
{7.6770138e-01},
{1.0325752e+00},
{1.1660632e+00},
{1.2543878e+00},
{1.1956003e+00},
{8.7571542e-01},
{6.1252082e-01},
{4.5699619e-01},
{7.4030148e-01},
{1.0017485e+00},
{9.8958385e-01},
{1.0949833e+00},
{1.1249612e+00},
{1.0658962e+00},
{9.5636335e-01},
{7.4498806e-01},
{7.2563393e-01},
{8.3530455e-01},
{1.0977307e+00},
{1.2314926e+00},
{1.3167422e+00},
{1.1172604e+00},
{7.6900341e-01},
{5.0549771e-01},
{4.4450990e-01},
{8.2808038e-01},
{9.6930396e-01},
{9.2772189e-01},
{1.1010458e+00},
{1.1366939e+00},
{1.1770457e+00},
{1.0282839e+00},
{7.8657093e-01},
{6.2340354e-01},
{6.3476140e-01},
{9.6941037e-01},
{1.1224823e+00},
{1.1576217e+00},
{1.2147442e+00},
{9.5631199e-01},
{7.3947874e-01},
{5.4187646e-01},
{6.8681641e-01},
{1.0131445e+00},
{1.0865854e+00},
{1.1414849e+00},
{1.1594630e+00},
{9.4411600e-01},
{7.7794034e-01},
{5.9741518e-01},
{7.7225037e-01},
{1.0587360e+00},
{1.1485933e+00},
{1.2344562e+00},
{1.1458853e+00},
{8.5415118e-01},
{6.2127481e-01},
{4.9756097e-01},
{8.0931406e-01},
{1.0430891e+00},
{1.0367506e+00},
{1.1608048e+00},
{1.0902522e+00},
{9.6945804e-01},
{7.9607346e-01},
{6.7035674e-01},
{8.2493000e-01},
{1.0685565e+00},
{1.1912458e+00},
{1.2916937e+00},
{1.1585237e+00},
{8.2653804e-01},
{5.5624667e-01},
{4.4364773e-01},
{7.8679896e-01},
{9.8947432e-01},
{9.5587476e-01},
{1.0976735e+00},
{1.1247419e+00},
{1.1220470e+00},
{9.9754667e-01},
{7.7406653e-01},
{6.7630712e-01},
{7.2445263e-01},
{1.0303591e+00},
{1.1801427e+00},
{1.2506686e+00},
{1.1989987e+00},
{8.7114663e-01},
{6.0602798e-01},
{4.5163741e-01},
{7.3553573e-01},
{9.9456302e-01},
{9.8098646e-01},
{1.0857031e+00},
{1.1289641e+00},
{1.0828069e+00},
{9.8224963e-01},
{7.5966277e-01},
{7.1111527e-01},
{7.8645769e-01},
{1.0673541e+00},
{1.2116816e+00},
{1.2990734e+00},
{1.1657704e+00},
{8.1779454e-01},
{5.4428063e-01},
{4.3047549e-01},
{7.7832590e-01},
{9.7551443e-01},
{9.3705618e-01},
{1.0807206e+00},
{1.1339189e+00},
{1.1551319e+00},
{1.0473610e+00},
{8.0021464e-01},
{6.5377513e-01},
{6.3810292e-01},
{9.6140773e-01},
{1.1354401e+00},
{1.1782311e+00},
{1.2323246e+00},
{9.5811452e-01},
{7.1400985e-01},
{5.1434848e-01},
{6.5876782e-01},
{9.9374878e-01},
{1.0532057e+00},
{1.0987291e+00},
{1.1569941e+00},
{9.9327584e-01},
{8.7055783e-01},
{6.6607204e-01},
{7.5154249e-01},
{9.9147904e-01},
{1.1618652e+00},
{1.2531013e+00},
{1.2445235e+00},
{9.2459418e-01},
{6.3924685e-01},
{4.4899067e-01},
{6.5946924e-01},
{9.7165260e-01},
{9.7791817e-01},
{1.0391898e+00},
{1.1412631e+00},
{1.0983393e+00},
{1.0493420e+00},
{8.0399406e-01},
{7.0992449e-01},
{7.0350356e-01},
{9.8952236e-01},
{1.1734521e+00},
{1.2538876e+00},
{1.2508500e+00},
{9.2423636e-01},
{6.3070030e-01},
{4.4136962e-01},
{6.4946966e-01},
{9.6197955e-01},
{9.6606332e-01},
{1.0237953e+00},
{1.1421522e+00},
{1.1183774e+00},
{1.0857179e+00},
{8.3157725e-01},
{7.0250588e-01},
{6.4917325e-01},
{9.2959329e-01},
{1.1407976e+00},
{1.2080261e+00},
{1.2684960e+00},
{9.9122930e-01},
{7.0220017e-01},
{4.8816416e-01},
{5.8586111e-01},
{9.4668329e-01},
{1.0087590e+00},
{1.0232927e+00},
{1.1559444e+00},
{1.0720927e+00},
{1.0286336e+00},
{7.9460252e-01},
{7.2093651e-01},
{7.6292167e-01},
{1.0248985e+00},
{1.1929944e+00},
{1.2920208e+00},
{1.2293181e+00},
{8.8290244e-01},
{5.8527613e-01},
{4.1855360e-01},
{6.9074767e-01},
{9.5977708e-01},
{9.3601019e-01},
{1.0268467e+00},
{1.1397909e+00},
{1.1581327e+00},
{1.1205956e+00},
{8.5280669e-01},
{6.7763483e-01},
{5.7983286e-01},
{8.6429914e-01},
{1.0976939e+00},
{1.1367408e+00},
{1.2374931e+00},
{1.0493382e+00},
{8.0588138e-01},
{5.9127581e-01},
{5.7929225e-01},
{9.2472931e-01},
{1.0849831e+00},
{1.0943246e+00},
{1.2041235e+00},
{1.0091917e+00},
{8.4987188e-01},
{6.3876182e-01},
{6.7747918e-01},
{9.7202288e-01},
{1.1405471e+00},
{1.1943368e+00},
{1.2338428e+00},
{9.4462558e-01},
{6.9738430e-01},
{4.9856007e-01},
{6.6823589e-01},
{9.9611375e-01},
{1.0393748e+00},
{1.0920087e+00},
{1.1489492e+00},
{1.0066576e+00},
{8.9589007e-01},
{6.8845995e-01},
{7.5294149e-01},
{9.6184563e-01},
{1.1555071e+00},
{1.2567316e+00},
{1.2751778e+00},
{9.6335246e-01},
{6.5830948e-01},
{4.4799275e-01},
{5.9888825e-01},
{9.4430988e-01},
{9.6837958e-01},
{9.9677549e-01},
{1.1434629e+00},
{1.1210684e+00},
{1.1148356e+00},
{8.6401556e-01},
{7.1420118e-01},
{6.2999725e-01},
{8.7537864e-01},
{1.1167941e+00},
{1.1896694e+00},
{1.2722030e+00},
{1.0454715e+00},
{7.4997564e-01},
{5.2383173e-01},
{5.3664145e-01},
{9.1117041e-01},
{1.0244044e+00},
{1.0151287e+00},
{1.1633675e+00},
{1.0695966e+00},
{1.0183575e+00},
{8.0087884e-01},
{7.1535296e-01},
{7.8553095e-01},
{1.0319822e+00},
{1.1937155e+00},
{1.2977947e+00},
{1.2217806e+00},
{8.7524512e-01},
{5.7851885e-01},
{4.1763620e-01},
{7.0155638e-01},
{9.6135267e-01},
{9.3393602e-01},
{1.0323424e+00},
{1.1394608e+00},
{1.1605598e+00},
{1.1161968e+00},
{8.4854339e-01},
{6.7338626e-01},
{5.7982826e-01},
{8.7043086e-01},
{1.0990834e+00},
{1.1357404e+00},
{1.2369383e+00},
{1.0441753e+00},
{8.0417958e-01},
{5.9002711e-01},
{5.8533894e-01},
{9.3051649e-01},
{1.0871279e+00},
{1.0984983e+00},
{1.2044155e+00},
{1.0039892e+00},
{8.4210523e-01},
{6.3124655e-01},
{6.8055115e-01},
{9.7935183e-01},
{1.1399170e+00},
{1.1935307e+00},
{1.2269029e+00},
{9.3758049e-01},
{6.9527243e-01},
{5.0033084e-01},
{6.8171448e-01},
{1.0034848e+00},
{1.0435853e+00},
{1.1027381e+00},
{1.1454200e+00},
{9.9636446e-01},
{8.7575610e-01},
{6.7572455e-01},
{7.6161436e-01},
{9.8751885e-01},
{1.1634852e+00},
{1.2613366e+00},
{1.2527158e+00},
{9.3041078e-01},
{6.3648886e-01},
{4.4289908e-01},
{6.4543043e-01},
{9.6271252e-01},
{9.6856037e-01},
{1.0237816e+00},
{1.1419384e+00},
{1.1143808e+00},
{1.0817980e+00},
{8.2952128e-01},
{7.0565231e-01},
{6.5745758e-01},
{9.3688510e-01},
{1.1457239e+00},
{1.2160319e+00},
{1.2696485e+00},
{9.8373294e-01},
{6.9141844e-01},
{4.7910268e-01},
{5.8995166e-01},
{9.4754885e-01},
{1.0004197e+00},
{1.0182334e+00},
{1.1527974e+00},
{1.0815601e+00},
{1.0462772e+00},
{8.0856012e-01},
{7.2009150e-01},
{7.3360533e-01},
{9.9726771e-01},
{1.1799228e+00},
{1.2752553e+00},
{1.2545993e+00},
{9.1834232e-01},
{6.1478702e-01},
{4.2718988e-01},
{6.4709630e-01},
{9.5321724e-01},
{9.4745071e-01},
{1.0082615e+00},
{1.1408283e+00},
{1.1449070e+00},
{1.1287695e+00},
{8.6506953e-01},
{6.9506068e-01},
{5.9077914e-01},
{8.5235412e-01},
{1.0981812e+00},
{1.1502068e+00},
{1.2457878e+00},
{1.0606248e+00},
{7.9743949e-01},
{5.7871635e-01},
{5.5597309e-01},
{9.0976389e-01},
{1.0678545e+00},
{1.0678466e+00},
{1.1926843e+00},
{1.0306506e+00},
{9.0068600e-01},
{6.8968565e-01},
{6.8293693e-01},
{9.2938386e-01},
{1.1389931e+00},
{1.2107723e+00},
{1.2742112e+00},
{9.9571305e-01},
{7.0900966e-01},
{4.8711002e-01},
{5.7843327e-01},
{9.4262722e-01},
{1.0068885e+00},
{1.0174671e+00},
{1.1549847e+00},
{1.0752383e+00},
{1.0382561e+00},
{8.0493588e-01},
{7.2297438e-01},
{7.5109199e-01},
{1.0099650e+00},
{1.1869526e+00},
{1.2859723e+00},
{1.2455314e+00},
{9.0247113e-01},
{5.9968946e-01},
{4.2056794e-01},
{6.6467784e-01},
{9.5461231e-01},
{9.3926954e-01},
{1.0128964e+00},
{1.1403075e+00},
{1.1549599e+00},
{1.1335031e+00},
{8.6584651e-01},
{6.8689154e-01},
{5.7663890e-01},
{8.4338430e-01},
{1.0904343e+00},
{1.1347127e+00},
{1.2344704e+00},
{1.0665197e+00},
{8.1831436e-01},
{6.0374950e-01},
{5.6609914e-01},
{9.0609488e-01},
{1.0837698e+00},
{1.0903349e+00},
{1.2068005e+00},
{1.0215332e+00},
{8.5834739e-01},
{6.4797779e-01},
{6.6278967e-01},
{9.5567807e-01},
{1.1370612e+00},
{1.1868501e+00},
{1.2426235e+00},
{9.6133342e-01},
{7.1189926e-01},
{5.0582127e-01},
{6.4642134e-01},
{9.8585814e-01},
{1.0428580e+00},
{1.0831242e+00},
{1.1568123e+00},
{1.0097872e+00},
{9.0412446e-01},
{6.9144384e-01},
{7.4452934e-01},
{9.5158341e-01},
{1.1512270e+00},
{1.2509608e+00},
{1.2807229e+00},
{9.7601316e-01},
{6.6959369e-01},
{4.5362019e-01},
{5.8397806e-01},
{9.3866865e-01},
{9.7211974e-01},
{9.9154526e-01},
{1.1440232e+00},
{1.1182310e+00},
{1.1159711e+00},
{8.6902975e-01},
{7.1839537e-01},
{6.3437176e-01},
{8.7058758e-01},
{1.1155207e+00},
{1.1929730e+00},
{1.2750340e+00},
{1.0511137e+00},
{7.5075715e-01},
{5.2249794e-01},
{5.2857771e-01},
{9.0479637e-01},
{1.0201490e+00},
{1.0078239e+00},
{1.1602784e+00},
{1.0762699e+00},
{1.0328279e+00},
{8.1754050e-01},
{7.1739458e-01},
{7.6521302e-01},
{1.0063562e+00},
{1.1829441e+00},
{1.2880301e+00},
{1.2505618e+00},
{9.0963666e-01},
{6.0373594e-01},
{4.2059572e-01},
{6.5552332e-01},
{9.5216646e-01},
{9.3915492e-01},
{1.0073402e+00},
{1.1402595e+00},
{1.1555473e+00},
{1.1400569e+00},
{8.7240707e-01},
{6.8972093e-01},
{5.7304964e-01},
{8.3155186e-01},
{1.0852244e+00},
{1.1309096e+00},
{1.2300408e+00},
{1.0756716e+00},
{8.2836037e-01},
{6.1503305e-01},
{5.6289537e-01},
{8.9596963e-01},
{1.0857176e+00},
{1.0933045e+00},
{1.2103223e+00},
{1.0266092e+00},
{8.5466111e-01},
{6.4485312e-01},
{6.5137619e-01},
{9.5039035e-01},
{1.1326982e+00},
{1.1771304e+00},
{1.2406618e+00},
{9.6686921e-01},
{7.2402007e-01},
{5.1657512e-01},
{6.4610863e-01},
{9.8716024e-01},
{1.0536774e+00},
{1.0920775e+00},
{1.1611436e+00},
{9.9694608e-01},
{8.7913715e-01},
{6.7096137e-01},
{7.4646346e-01},
{9.8211586e-01},
{1.1596887e+00},
{1.2506463e+00},
{1.2528610e+00},
{9.3574881e-01},
{6.4708026e-01},
{4.5058777e-01},
{6.4326949e-01},
{9.6585146e-01},
{9.7825812e-01},
{1.0299115e+00},
{1.1430802e+00},
{1.1005190e+00},
{1.0609387e+00},
{8.1379890e-01},
{7.1122428e-01},
{6.9156533e-01},
{9.7259709e-01},
{1.1655186e+00},
{1.2452699e+00},
{1.2619116e+00},
{9.4482161e-01},
{6.4801701e-01},
{4.4907240e-01},
{6.2485209e-01},
{9.5564190e-01},
{9.7288683e-01},
{1.0147847e+00},
{1.1443809e+00},
{1.1120870e+00},
{1.0877802e+00},
{8.3639407e-01},
{7.0935594e-01},
{6.5684224e-01},
{9.2695542e-01},
{1.1418485e+00},
{1.2155793e+00},
{1.2744439e+00},
{9.9526406e-01},
{6.9858658e-01},
{4.8225678e-01},
{5.7551868e-01},
{9.3953863e-01},
{1.0000682e+00},
{1.0105854e+00},
{1.1531271e+00},
{1.0852683e+00},
{1.0562598e+00},
{8.2024340e-01},
{7.2070006e-01},
{7.2200828e-01},
{9.7907123e-01},
{1.1713283e+00},
{1.2668075e+00},
{1.2685662e+00},
{9.4120299e-01},
{6.3302397e-01},
{4.3403723e-01},
{6.1877701e-01},
{9.4622798e-01},
{9.5365292e-01},
{9.9618857e-01},
{1.1414351e+00},
{1.1389950e+00},
{1.1349258e+00},
{8.7553815e-01},
{7.0489527e-01},
{5.9575516e-01},
{8.3938416e-01},
{1.0946321e+00},
{1.1549299e+00},
{1.2478627e+00},
{1.0727433e+00},
{8.0007945e-01},
{5.7879852e-01},
{5.4142544e-01},
{8.9584895e-01},
{1.0605852e+00},
{1.0558398e+00},
{1.1875826e+00},
{1.0434789e+00},
{9.2439267e-01},
{7.1612758e-01},
{6.8218882e-01},
{9.0102182e-01},
{1.1275375e+00},
{1.2109971e+00},
{1.2892493e+00},
{1.0360675e+00},
{7.3214326e-01},
{4.9603224e-01},
{5.2859768e-01},
{9.0839931e-01},
{9.9967438e-01},
{9.8761933e-01},
{1.1495807e+00},
{1.0955451e+00},
{1.0837190e+00},
{8.6306190e-01},
{7.2864571e-01},
{6.9570443e-01},
{9.1466309e-01},
{1.1416079e+00},
{1.2423455e+00},
{1.2979661e+00},
{1.0158501e+00},
{6.9306586e-01},
{4.6540187e-01},
{5.3429520e-01},
{9.1176535e-01},
{9.7214664e-01},
{9.6587930e-01},
{1.1395672e+00},
{1.1254350e+00},
{1.1438751e+00},
{9.1306791e-01},
{7.3239337e-01},
{6.1798523e-01},
{7.9591503e-01},
{1.0777494e+00},
{1.1677842e+00},
{1.2502967e+00},
{1.1151699e+00},
{8.1642087e-01},
{5.8442000e-01},
{5.0055038e-01},
{8.4650946e-01},
{1.0385843e+00},
{1.0234916e+00},
{1.1635033e+00},
{1.0803258e+00},
{9.9050756e-01},
{8.0394987e-01},
{6.9093478e-01},
{8.1258118e-01},
{1.0510811e+00},
{1.1928214e+00},
{1.2964341e+00},
{1.1903709e+00},
{8.5006682e-01},
{5.6609363e-01},
{4.2789827e-01},
{7.4508986e-01},
{9.7546850e-01},
{9.4304490e-01},
{1.0657257e+00},
{1.1344752e+00},
{1.1448680e+00},
{1.0604924e+00},
{8.0946851e-01},
{6.6897242e-01},
{6.3751840e-01},
{9.5203759e-01},
{1.1383222e+00},
{1.1857882e+00},
{1.2429358e+00},
{9.6640521e-01},
{7.0943525e-01},
{5.0606242e-01},
{6.3814517e-01},
{9.8050653e-01},
{1.0400252e+00},
{1.0768445e+00},
{1.1586073e+00},
{1.0171349e+00},
{9.1652446e-01},
{7.0015343e-01},
{7.3826470e-01},
{9.3314098e-01},
{1.1436325e+00},
{1.2463125e+00},
{1.2921798e+00},
{1.0004180e+00},
{6.8706555e-01},
{4.6091370e-01},
{5.5360860e-01},
{9.2333217e-01},
{9.7324082e-01},
{9.7644851e-01},
{1.1422917e+00},
{1.1211038e+00},
{1.1312591e+00},
{8.9312684e-01},
{7.2669146e-01},
{6.2634170e-01},
{8.3089176e-01},
{1.0962164e+00},
{1.1808500e+00},
{1.2661262e+00},
{1.0871224e+00},
{7.8484345e-01},
{5.5241172e-01},
{5.0720010e-01},
{8.7433135e-01},
{1.0293171e+00},
{1.0110677e+00},
{1.1612664e+00},
{1.0781429e+00},
{1.0169296e+00},
{8.1818733e-01},
{7.0825239e-01},
{7.8555548e-01},
{1.0202406e+00},
{1.1861871e+00},
{1.2927766e+00},
{1.2345608e+00},
{8.9196638e-01},
{5.9165129e-01},
{4.1993302e-01},
{6.8133251e-01},
{9.5902336e-01},
{9.3840491e-01},
{1.0229760e+00},
{1.1397483e+00},
{1.1549910e+00},
{1.1221729e+00},
{8.5521466e-01},
{6.8171599e-01},
{5.8253142e-01},
{8.6249156e-01},
{1.0984374e+00},
{1.1402897e+00},
{1.2397438e+00},
{1.0509675e+00},
{8.0267914e-01},
{5.8719202e-01},
{5.7423177e-01},
{9.2225657e-01},
{1.0807070e+00},
{1.0878295e+00},
{1.2011335e+00},
{1.0137713e+00},
{8.6200406e-01},
{6.5063788e-01},
{6.7962388e-01},
{9.6415425e-01},
{1.1427244e+00},
{1.2001432e+00},
{1.2435862e+00},
{9.5255718e-01},
{6.9548843e-01},
{4.9228114e-01},
{6.4975070e-01},
{9.8475452e-01},
{1.0290189e+00},
{1.0735305e+00},
{1.1514964e+00},
{1.0265685e+00},
{9.3450570e-01},
{7.1609613e-01},
{7.4066401e-01},
{9.0672547e-01},
{1.1323663e+00},
{1.2457251e+00},
{1.3080265e+00},
{1.0368462e+00},
{7.0922252e-01},
{4.6922499e-01},
{5.0971665e-01},
{8.9530783e-01},
{9.6897032e-01},
{9.5144881e-01},
{1.1327578e+00},
{1.1313397e+00},
{1.1615669e+00},
{9.4490349e-01},
{7.4435097e-01},
{6.0879030e-01},
{7.4201167e-01},
{1.0475442e+00},
{1.1489683e+00},
{1.2189341e+00},
{1.1527726e+00},
{8.6373864e-01},
{6.3765290e-01},
{5.0482479e-01},
{7.9976041e-01},
{1.0463811e+00},
{1.0486243e+00},
{1.1650654e+00},
{1.0937932e+00},
{9.5242412e-01},
{7.7776887e-01},
{6.5337298e-01},
{8.3412675e-01},
{1.0842165e+00},
{1.1876958e+00},
{1.2860798e+00},
{1.1277640e+00},
{8.0705117e-01},
{5.4933954e-01},
{4.6409423e-01},
{8.2407983e-01},
{1.0029510e+00},
{9.7182671e-01},
{1.1248927e+00},
{1.1108497e+00},
{1.0927585e+00},
{9.3345740e-01},
{7.4563087e-01},
{6.9815644e-01},
{8.2911579e-01},
{1.0975320e+00},
{1.2225208e+00},
{1.3041931e+00},
{1.1107874e+00},
{7.6954799e-01},
{5.1381033e-01},
{4.5656108e-01},
{8.3803614e-01},
{9.8059578e-01},
{9.4343837e-01},
{1.1139527e+00},
{1.1272959e+00},
{1.1505047e+00},
{9.8802150e-01},
{7.6846480e-01},
{6.4059710e-01},
{7.0404249e-01},
{1.0213995e+00},
{1.1571646e+00},
{1.2167098e+00},
{1.1922700e+00},
{8.8891917e-01},
{6.4610240e-01},
{4.8423413e-01},
{7.4237703e-01},
{1.0194305e+00},
{1.0263817e+00},
{1.1223002e+00},
{1.1218476e+00},
{1.0047937e+00},
{8.6916869e-01},
{6.8761628e-01},
{7.7272726e-01},
{9.8680374e-01},
{1.1662112e+00},
{1.2718586e+00},
{1.2599891e+00},
{9.3374084e-01},
{6.3070745e-01},
{4.3512068e-01},
{6.3400337e-01},
{9.5387123e-01},
{9.5725486e-01},
{1.0083783e+00},
{1.1414776e+00},
{1.1320033e+00},
{1.1156485e+00},
{8.5730304e-01},
{7.0257566e-01},
{6.1385070e-01},
{8.7645096e-01},
{1.1135407e+00},
{1.1743915e+00},
{1.2617479e+00},
{1.0416530e+00},
{7.6141001e-01},
{5.3947941e-01},
{5.5263397e-01},
{9.2079176e-01},
{1.0423877e+00},
{1.0391593e+00},
{1.1741828e+00},
{1.0475738e+00},
{9.6528247e-01},
{7.4824631e-01},
{7.0765109e-01},
{8.6076413e-01},
{1.1015853e+00},
{1.2180296e+00},
{1.3087379e+00},
{1.1050105e+00},
{7.7144492e-01},
{5.1073157e-01},
{4.5959721e-01},
{8.4138746e-01},
{9.8058403e-01},
{9.4385625e-01},
{1.1155190e+00},
{1.1265194e+00},
{1.1505430e+00},
{9.8480260e-01},
{7.6753361e-01},
{6.3998096e-01},
{7.0790430e-01},
{1.0242649e+00},
{1.1582776e+00},
{1.2187970e+00},
{1.1895283e+00},
{8.8519343e-01},
{6.4254017e-01},
{4.8336487e-01},
{7.4700679e-01},
{1.0202300e+00},
{1.0250636e+00},
{1.1237335e+00},
{1.1201468e+00},
{1.0057497e+00},
{8.6867488e-01},
{6.8876595e-01},
{7.7314271e-01},
{9.8646241e-01},
{1.1663720e+00},
{1.2725046e+00},
{1.2607985e+00},
{9.3432428e-01},
{6.3055002e-01},
{4.3462242e-01},
{6.3267663e-01},
{9.5306639e-01},
{9.5647018e-01},
{1.0069834e+00},
{1.1414283e+00},
{1.1332454e+00},
{1.1183329e+00},
{8.5964718e-01},
{7.0259273e-01},
{6.1086847e-01},
{8.7145976e-01},
{1.1109278e+00},
{1.1713319e+00},
{1.2600859e+00},
{1.0459933e+00},
{7.6697548e-01},
{5.4492361e-01},
{5.5082792e-01},
{9.1787603e-01},
{1.0454451e+00},
{1.0416026e+00},
{1.1762039e+00},
{1.0460129e+00},
{9.5856257e-01},
{7.4291843e-01},
{7.0478730e-01},
{8.6904294e-01},
{1.1074327e+00},
{1.2185136e+00},
{1.3067057e+00},
{1.0916662e+00},
{7.6226341e-01},
{5.0599582e-01},
{4.7004755e-01},
{8.5459323e-01},
{9.8292600e-01},
{9.4923741e-01},
{1.1230180e+00},
{1.1230727e+00},
{1.1436380e+00},
{9.6517685e-01},
{7.5906667e-01},
{6.4229098e-01},
{7.3739829e-01},
{1.0441989e+00},
{1.1689429e+00},
{1.2377221e+00},
{1.1702644e+00},
{8.5776744e-01},
{6.1283507e-01},
{4.7508171e-01},
{7.7650778e-01},
{1.0199875e+00},
{1.0098181e+00},
{1.1277738e+00},
{1.1115547e+00},
{1.0256012e+00},
{8.8156161e-01},
{7.0600708e-01},
{7.6171841e-01},
{9.5613909e-01},
{1.1569502e+00},
{1.2691300e+00},
{1.2887802e+00},
{9.7384680e-01},
{6.5493328e-01},
{4.3953117e-01},
{5.7648851e-01},
{9.2999054e-01},
{9.5436230e-01},
{9.7297008e-01},
{1.1391325e+00},
{1.1416064e+00},
{1.1590944e+00},
{9.0850247e-01},
{7.1895834e-01},
{5.8564424e-01},
{7.8301903e-01},
{1.0675300e+00},
{1.1390766e+00},
{1.2237314e+00},
{1.1158432e+00},
{8.4615038e-01},
{6.2929929e-01},
{5.3024321e-01},
{8.4771779e-01},
{1.0679772e+00},
{1.0707805e+00},
{1.1942615e+00},
{1.0628072e+00},
{9.0277875e-01},
{7.0464579e-01},
{6.4124982e-01},
{8.9445063e-01},
{1.1236998e+00},
{1.1838208e+00},
{1.2721938e+00},
{1.0302053e+00},
{7.4928918e-01},
{5.2054789e-01},
{5.5300914e-01},
{9.2450978e-01},
{1.0290172e+00},
{1.0254177e+00},
{1.1658431e+00},
{1.0585030e+00},
{9.9937151e-01},
{7.7845423e-01},
{7.1727834e-01},
{8.1473462e-01},
{1.0635825e+00},
{1.2081434e+00},
{1.3087387e+00},
{1.1774543e+00},
{8.2930325e-01},
{5.4568406e-01},
{4.2252006e-01},
{7.6155262e-01},
{9.6902023e-01},
{9.2946430e-01},
{1.0663917e+00},
{1.1373062e+00},
{1.1667915e+00},
{1.0769457e+00},
{8.1823128e-01},
{6.5190054e-01},
{6.0082155e-01},
{9.2122516e-01},
{1.1147361e+00},
{1.1467530e+00},
{1.2352757e+00},
{9.9993521e-01},
{7.6890308e-01},
{5.5857496e-01},
{6.2725763e-01},
{9.7318317e-01},
{1.0840117e+00},
{1.1108847e+00},
{1.1857549e+00},
{9.7757101e-01},
{8.2394503e-01},
{6.1941336e-01},
{7.2410931e-01},
{1.0158088e+00},
{1.1491174e+00},
{1.2188178e+00},
{1.2007209e+00},
{8.9859946e-01},
{6.5456633e-01},
{4.8391373e-01},
{7.3135136e-01},
{1.0176181e+00},
{1.0277501e+00},
{1.1176213e+00},
{1.1248859e+00},
{1.0041498e+00},
{8.7364691e-01},
{6.8812425e-01},
{7.7228478e-01},
{9.8423833e-01},
{1.1655135e+00},
{1.2710944e+00},
{1.2619166e+00},
{9.3651452e-01},
{6.3284215e-01},
{4.3578889e-01},
{6.3021849e-01},
{9.5269846e-01},
{9.5770175e-01},
{1.0064613e+00},
{1.1416014e+00},
{1.1318041e+00},
{1.1173687e+00},
{8.5933718e-01},
{7.0364317e-01},
{6.1326177e-01},
{8.7318554e-01},
{1.1121947e+00},
{1.1737687e+00},
{1.2616019e+00},
{1.0447114e+00},
{7.6386287e-01},
{5.4149208e-01},
{5.5006680e-01},
{9.1816862e-01},
{1.0427545e+00},
{1.0385725e+00},
{1.1744959e+00},
{1.0484442e+00},
{9.6566520e-01},
{7.4941645e-01},
{7.0654030e-01},
{8.5978495e-01},
{1.1007328e+00},
{1.2172148e+00},
{1.3084224e+00},
{1.1066295e+00},
{7.7306455e-01},
{5.1195750e-01},
{4.5881820e-01},
{8.3991753e-01},
{9.8090418e-01},
{9.4402667e-01},
{1.1150495e+00},
{1.1263864e+00},
{1.1498512e+00},
{9.8537193e-01},
{7.6788821e-01},
{6.4101222e-01},
{7.0798171e-01},
{1.0241973e+00},
{1.1588402e+00},
{1.2194902e+00},
{1.1900225e+00},
{8.8503290e-01},
{6.4170616e-01},
{4.8247262e-01},
{7.4633801e-01},
{1.0194608e+00},
{1.0238758e+00},
{1.1225142e+00},
{1.1204876e+00},
{1.0078894e+00},
{8.7195748e-01},
{6.9071922e-01},
{7.7128559e-01},
{9.8171063e-01},
{1.1648359e+00},
{1.2716060e+00},
{1.2653194e+00},
{9.4044785e-01},
{6.3462126e-01},
{4.3541986e-01},
{6.2397592e-01},
{9.4996429e-01},
{9.5654324e-01},
{1.0018949e+00},
{1.1415375e+00},
{1.1340702e+00},
{1.1242483e+00},
{8.6601272e-01},
{7.0479002e-01},
{6.0700568e-01},
{8.6003770e-01},
{1.1056434e+00},
{1.1671804e+00},
{1.2575999e+00},
{1.0561104e+00},
{7.7723774e-01},
{5.5451638e-01},
{5.4470017e-01},
{9.0953268e-01},
{1.0489983e+00},
{1.0432715e+00},
{1.1787522e+00},
{1.0466535e+00},
{9.5253913e-01},
{7.3976965e-01},
{6.9910727e-01},
{8.7457184e-01},
{1.1111747e+00},
{1.2168058e+00},
{1.3037145e+00},
{1.0820947e+00},
{7.5710433e-01},
{5.0441461e-01},
{4.7923251e-01},
{8.6434525e-01},
{9.8624111e-01},
{9.5555561e-01},
{1.1291297e+00},
{1.1187232e+00},
{1.1339455e+00},
{9.4624297e-01},
{7.5185473e-01},
{6.4886735e-01},
{7.6947931e-01},
{1.0640658e+00},
{1.1819490e+00},
{1.2581006e+00},
{1.1479357e+00},
{8.2795383e-01},
{5.8084914e-01},
{4.6968322e-01},
{8.0540679e-01},
{1.0147037e+00},
{9.9267552e-01},
{1.1297166e+00},
{1.1074520e+00},
{1.0535732e+00},
{8.9989357e-01},
{7.2414622e-01},
{7.3710872e-01},
{9.0842211e-01},
{1.1372436e+00},
{1.2550645e+00},
{1.3118017e+00},
{1.0309191e+00},
{6.9715233e-01},
{4.6014528e-01},
{5.1093338e-01},
{8.9602085e-01},
{9.6063866e-01},
{9.4454964e-01},
{1.1299822e+00},
{1.1400660e+00},
{1.1784050e+00},
{9.5942169e-01},
{7.4592605e-01},
{5.9179210e-01},
{7.0833096e-01},
{1.0269127e+00},
{1.1280089e+00},
{1.1874924e+00},
{1.1668412e+00},
{8.9924653e-01},
{6.8916581e-01},
{5.3047422e-01},
{7.7534076e-01},
{1.0549099e+00},
{1.0847312e+00},
{1.1818109e+00},
{1.1071479e+00},
{9.0609669e-01},
{7.2016981e-01},
{5.9922569e-01},
{8.4631545e-01},
{1.1007330e+00},
{1.1539455e+00},
{1.2548057e+00},
{1.0707369e+00},
{7.9955359e-01},
{5.7211183e-01},
{5.3729384e-01},
{8.9511329e-01},
{1.0559775e+00},
{1.0482488e+00},
{1.1833103e+00},
{1.0467011e+00},
{9.3892077e-01},
{7.3109756e-01},
{6.8881096e-01},
{8.8724202e-01},
{1.1196405e+00},
{1.2137632e+00},
{1.2969341e+00},
{1.0598913e+00},
{7.4476987e-01},
{5.0025043e-01},
{5.0175473e-01},
{8.8612613e-01},
{9.9295902e-01},
{9.7027740e-01},
{1.1406663e+00},
{1.1082236e+00},
{1.1111857e+00},
{9.0519862e-01},
{7.3853971e-01},
{6.6752554e-01},
{8.4076984e-01},
{1.1041197e+00},
{1.2107499e+00},
{1.2910787e+00},
{1.0889124e+00},
{7.6218879e-01},
{5.1854183e-01},
{4.8212027e-01},
{8.6343438e-01},
{9.9664176e-01},
{9.6786348e-01},
{1.1354766e+00},
{1.1100558e+00},
{1.1078273e+00},
{9.1950063e-01},
{7.4186814e-01},
{6.7554741e-01},
{8.2771704e-01},
{1.0970261e+00},
{1.2115055e+00},
{1.2926801e+00},
{1.1044430e+00},
{7.7268354e-01},
{5.2367729e-01},
{4.6962326e-01},
{8.4775775e-01},
{9.9325612e-01},
{9.6094332e-01},
{1.1271528e+00},
{1.1156670e+00},
{1.1183008e+00},
{9.4351589e-01},
{7.5035067e-01},
{6.6773972e-01},
{7.8886291e-01},
{1.0750322e+00},
{1.1971741e+00},
{1.2770426e+00},
{1.1387571e+00},
{8.0837143e-01},
{5.5509702e-01},
{4.5955065e-01},
{8.1398530e-01},
{1.0008314e+00},
{9.7057086e-01},
{1.1195985e+00},
{1.1149111e+00},
{1.0957539e+00},
{9.4451157e-01},
{7.4819097e-01},
{6.9474604e-01},
{8.1250200e-01},
{1.0876804e+00},
{1.2158509e+00},
{1.2988266e+00},
{1.1270851e+00},
{7.8595861e-01},
{5.2690470e-01},
{4.5032342e-01},
{8.2266007e-01},
{9.8371303e-01},
{9.4630598e-01},
{1.1090276e+00}};

    private static readonly Dataset defaultDataset;
    private static readonly IEnumerable<string> defaultAllowedInputVariables;
    private static readonly string defaultTargetVariable;

    private static readonly TimeSeriesPrognosisProblemData emptyProblemData;
    public static TimeSeriesPrognosisProblemData EmptyProblemData {
      get { return emptyProblemData; }
    }

    static TimeSeriesPrognosisProblemData() {
      defaultDataset = new Dataset(new string[] { "x" }, mackey_glass_17);
      defaultDataset.Name = "Mackey-Glass (t=17) Time Series Benchmark Dataset";
      defaultAllowedInputVariables = new List<string>() { "x" };
      defaultTargetVariable = "x";

      var problemData = new TimeSeriesPrognosisProblemData();
      problemData.Parameters.Clear();
      problemData.Name = "Empty Time-Series Prognosis ProblemData";
      problemData.Description = "This ProblemData acts as place holder before the correct problem data is loaded.";
      problemData.isEmpty = true;

      problemData.Parameters.Add(new FixedValueParameter<Dataset>(DatasetParameterName, "", new Dataset()));
      problemData.Parameters.Add(new FixedValueParameter<ReadOnlyCheckedItemList<StringValue>>(InputVariablesParameterName, ""));
      problemData.Parameters.Add(new FixedValueParameter<IntRange>(TrainingPartitionParameterName, "", (IntRange)new IntRange(0, 0).AsReadOnly()));
      problemData.Parameters.Add(new FixedValueParameter<IntRange>(TestPartitionParameterName, "", (IntRange)new IntRange(0, 0).AsReadOnly()));
      problemData.Parameters.Add(new ConstrainedValueParameter<StringValue>(TargetVariableParameterName, new ItemSet<StringValue>()));
      emptyProblemData = problemData;
    }
    #endregion

    public ConstrainedValueParameter<StringValue> TargetVariableParameter {
      get { return (ConstrainedValueParameter<StringValue>)Parameters[TargetVariableParameterName]; }
    }
    public string TargetVariable {
      get { return TargetVariableParameter.Value.Value; }
    }

    [StorableConstructor]
    protected TimeSeriesPrognosisProblemData(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterParameterEvents();
    }

    protected TimeSeriesPrognosisProblemData(TimeSeriesPrognosisProblemData original, Cloner cloner)
      : base(original, cloner) {
      RegisterParameterEvents();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      if (this == emptyProblemData) return emptyProblemData;
      return new TimeSeriesPrognosisProblemData(this, cloner);
    }

    public TimeSeriesPrognosisProblemData()
      : this(defaultDataset, defaultAllowedInputVariables, defaultTargetVariable) {
    }

    public TimeSeriesPrognosisProblemData(Dataset dataset, IEnumerable<string> allowedInputVariables, string targetVariable)
      : base(dataset, allowedInputVariables) {
      var variables = InputVariables.Select(x => x.AsReadOnly()).ToList();
      Parameters.Add(new ConstrainedValueParameter<StringValue>(TargetVariableParameterName, new ItemSet<StringValue>(variables), variables.Where(x => x.Value == targetVariable).First()));
      RegisterParameterEvents();
    }

    private void RegisterParameterEvents() {
      TargetVariableParameter.ValueChanged += TargetVariableParameter_ValueChanged;
    }

    private void TargetVariableParameter_ValueChanged(object sender, EventArgs e) {
      OnChanged();
    }

    #region Import from file
    public static TimeSeriesPrognosisProblemData ImportFromFile(string fileName) {
      TableFileParser csvFileParser = new TableFileParser();
      csvFileParser.Parse(fileName);

      Dataset dataset = new Dataset(csvFileParser.VariableNames, csvFileParser.Values);
      dataset.Name = Path.GetFileName(fileName);

      TimeSeriesPrognosisProblemData problemData = new TimeSeriesPrognosisProblemData(dataset, dataset.DoubleVariables, dataset.DoubleVariables.First());
      problemData.Name = "Data imported from " + Path.GetFileName(fileName);
      return problemData;
    }
    #endregion
  }
}
