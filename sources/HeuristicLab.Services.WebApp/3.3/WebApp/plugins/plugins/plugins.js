var appPluginsPlugin = app.registerPlugin('plugins');
(function () {
    var plugin = appPluginsPlugin;
    plugin.dependencies = ['ngResource', 'ui.bootstrap', 'tableSort'];
    plugin.files = [
        'pluginsService.js',
        'pluginsCtrl.js',
        'pluginsExceptionCtrl.js'
    ];
    plugin.view = 'plugins.cshtml';
    plugin.controller = 'app.plugins.ctrl';
    plugin.routes = [];

    var menu = app.getMenu();
    var section = menu.getSection('Administration', -1);
    section.addEntry({
        index: 10,
        name: 'Plugins',
        route: '#/plugins',
        icon: 'glyphicon glyphicon-wrench',
        entries: []
    });
})();