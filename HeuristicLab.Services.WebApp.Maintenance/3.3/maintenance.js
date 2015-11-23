var appMaintenancePlugin = app.registerPlugin('maintenance');
(function () {
    var plugin = appMaintenancePlugin;
    plugin.dependencies = ['ngResource', 'ui.knob', 'ui.bootstrap', 'tableSort'];
    plugin.files = [
        'WebApp/services/spaceUsageService.js',
        'WebApp/services/pluginService.js',
        'WebApp/services/facttaskService.js',
        'WebApp/services/factclientinfoService.js',
        'WebApp/plugin/pluginCtrl.js',
        'WebApp/facttask/facttaskCtrl.js',
        'WebApp/factclientinfo/factclientinfoCtrl.js',
        'WebApp/spaceUsage/spaceUsageCtrl.js'
    ];
    plugin.view = 'WebApp/spaceUsage/spaceUsage.cshtml';
    plugin.controller = 'app.maintenance.spaceUsageCtrl';
    plugin.routes = [
        new Route('spaceusage', 'WebApp/spaceUsage/spaceUsage.cshtml', 'app.maintenance.spaceUsageCtrl'),
        new Route('facttask', 'WebApp/facttask/facttask.cshtml', 'app.maintenance.facttaskCtrl'),
        new Route('factclientinfo', 'WebApp/factclientinfo/factclientinfo.cshtml', 'app.maintenance.factclientinfoCtrl'),
        new Route('plugin', 'WebApp/plugin/plugin.cshtml', 'app.maintenance.pluginCtrl')
    ];
    var menu = app.getMenu();
    var section = menu.getSection('Administration', -1);
    section.addEntry({
        name: 'Maintenance',
        route: '#/maintenance',
        icon: 'glyphicon glyphicon-cog',
        entries: []
    });
})();

