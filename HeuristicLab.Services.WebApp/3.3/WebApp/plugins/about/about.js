var appAboutPlugin = app.registerPlugin('about');
(function () {
    var plugin = appAboutPlugin;
    plugin.dependencies = ['ngResource', 'ui.bootstrap'];
    plugin.files = ['aboutCtrl.js'];
    plugin.view = 'about.cshtml';
    plugin.controller = 'app.about.ctrl';
    plugin.routes = [];

    var menu = app.getMenu();
    var section = menu.getSection('Menu', -1);
    section.addEntry({
        index: 10000,
        name: 'About',
        route: '#/about',
        icon: 'glyphicon glyphicon-info-sign',
        entries: []
    });
})();