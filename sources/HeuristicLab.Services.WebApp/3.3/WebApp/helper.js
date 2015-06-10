var isDefined = function(variable) {
    return !(variable === undefined || variable === null);
};

function zeropad(n) { return ("0" + n).slice(-2); }

Number.prototype.zeropad = function (n) {
    return (new Array(n + 1).join("0") + this).slice(-n);
};

var ConvertDate = function (date) {
    var day = date.getDate().zeropad(2);
    var month = (1 + date.getMonth()).zeropad(2);
    var year = date.getFullYear();
    var hour = (date.getHours() - 2).zeropad(2);
    var minute = date.getMinutes().zeropad(2);
    var second = date.getSeconds().zeropad(2);
    return year + '-' + month + '-' + day + 'T' + hour + ':' + minute + ':' + second + '.000Z';
};

var ConvertFromDate = function (date) {
    var day = date.getDate().zeropad(2);     
    var month = (1 + date.getMonth()).zeropad(2);
    var year = date.getFullYear();
    return year + '-' + month + '-' + day + 'T00:00:00.000Z';
};

var ConvertToDate = function (date) {
    var day = date.getDate().zeropad(2);
    var month = (1 + date.getMonth()).zeropad(2);
    var year = date.getFullYear();
    return year + '-' + month + '-' + day + 'T23:59:59.000Z';
};


var CSharpDateToString = function (datetime) {
    var date = new Date(Date.parse(datetime));
    var day = date.getDate().zeropad(2);
    var month = (1 + date.getMonth()).zeropad(2);
    var year = date.getFullYear();
    var hour = date.getHours().zeropad(2);
    var minute = date.getMinutes().zeropad(2);
    var second = date.getSeconds().zeropad(2);
    return day + '.' + month + '.' + year + ' ' + hour + ':' + minute + ':' + second;
};