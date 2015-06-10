var appStatusPlugin = app.registerPlugin('status');
(function () {
    var plugin = appStatusPlugin;
    plugin.dependencies = ['ngResource', 'ui.knob', 'ui.bootstrap'];
    plugin.files = [
        'WebApp/status.css',
        'WebApp/services/statusService.js',
        'WebApp/status/statusCtrl.js',
        'WebApp/history/historyCtrl.js'
    ];
    plugin.view = 'WebApp/status/status.cshtml';
    plugin.controller = 'app.status.ctrl';
    plugin.routes = [
        new Route('history', 'WebApp/history/history.cshtml', 'app.status.historyCtrl')
    ];
    var menu = app.getMenu();
    var section = menu.getSection('Menü', 1);
    section.addEntry({
        name: 'Status',
        route: '#/status',
        icon: 'glyphicon glyphicon-dashboard',
        entries: [{
            name: 'History',
            route: '#/status/history',
            icon: 'glyphicon glyphicon-stats'
        }]
    });
})();

