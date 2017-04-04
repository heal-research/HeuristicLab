var appStatisticsPlugin = app.registerPlugin('statistics');
(function () {
    var plugin = appStatisticsPlugin;
    plugin.dependencies = ['ngResource', 'ui.knob', 'ui.bootstrap', 'ngFitText', 'tableSort'];
    plugin.files = [
        'WebApp/statistics.css',
        'WebApp/services/statisticsService.js',
        'WebApp/services/clientService.js',
        'WebApp/services/exceptionService.js',
        'WebApp/services/groupService.js',
        'WebApp/services/jobService.js',
        'WebApp/services/taskService.js',
        'WebApp/services/userService.js',
        'WebApp/jobs/jobsCtrl.js',
        'WebApp/jobs/details/jobDetailsCtrl.js',
        'WebApp/jobs/details/jobTaskDetailsDialogCtrl.js',
        'WebApp/clients/clientsCtrl.js',
        'WebApp/clients/details/clientDetailsCtrl.js',
        'WebApp/clients/details/clientTaskDetailsDialogCtrl.js',
        'WebApp/users/usersCtrl.js',
        'WebApp/users/details/userDetailsCtrl.js',
        'WebApp/groups/groupsCtrl.js',
        'WebApp/groups/details/groupDetailsCtrl.js',
        'WebApp/exceptions/exceptionsCtrl.js',
        'WebApp/exceptions/exceptionDetailsDialogCtrl.js'
    ];
    plugin.view = 'WebApp/jobs/jobs.cshtml';
    plugin.controller = 'app.statistics.jobsCtrl';
    plugin.routes = [
        new Route('jobs', 'WebApp/jobs/jobs.cshtml', 'app.statistics.jobsCtrl'),
        new Route('jobs/:id', 'WebApp/jobs/details/jobDetails.cshtml', 'app.statistics.jobDetailsCtrl'),
        new Route('clients', 'WebApp/clients/clients.cshtml', 'app.statistics.clientsCtrl'),
        new Route('clients/:id', 'WebApp/clients/details/clientDetails.cshtml', 'app.statistics.clientDetailsCtrl'),
        new Route('users', 'WebApp/users/users.cshtml', 'app.statistics.usersCtrl'),
        new Route('users/:id', 'WebApp/users/details/userDetails.cshtml', 'app.statistics.userDetailsCtrl'),
        new Route('groups', 'WebApp/groups/groups.cshtml', 'app.statistics.groupsCtrl'),
        new Route('groups/:id', 'WebApp/groups/details/groupDetails.cshtml', 'app.statistics.groupDetailsCtrl'),
        new Route('exceptions', 'WebApp/exceptions/exceptions.cshtml', 'app.statistics.exceptionsCtrl')
    ];
    var menu = app.getMenu();
    var section = menu.getSection('Menu', 1);
    section.addEntry({
        name: 'Statistics',
        route: '#/statistics',
        icon: 'glyphicon glyphicon-stats',
        entries: []
    });
})();

