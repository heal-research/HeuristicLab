var isDefined = function(variable) {
    return !(variable === undefined || variable === null);
};

function zeropad(n) { return ("0" + n).slice(-2); }

Number.prototype.zeropad = function (n) {
    return (new Array(n + 1).join("0") + this).slice(-n);
};

Number.prototype.toHHMMSS = function() {
    d = this;
    d = Number(d);
    var years = Math.floor(d / 31536000);
    var days = Math.floor(d / 86400 % 365);
    var h = Math.floor(d / 3600 % 24);
    var m = Math.floor(d % 3600 / 60);
    var s = Math.floor(d % 3600 % 60);
    var timeStr = "";
    if (years > 0) {
        timeStr = years + "y ";
    }
    if (days > 0) {
        timeStr = timeStr + days + "d ";
    }
    return timeStr + h.zeropad(2) + ":" + m.zeropad(2) + ":" + s.zeropad(2);
};

var ConvertDate = function (date) {
    var day = date.getDate().zeropad(2);
    var month = (1 + date.getMonth()).zeropad(2);
    var year = date.getFullYear();
    var hour = (date.getHours() + date.getTimezoneOffset() / 60).zeropad(2);
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

var decryptString = function(s) {
    return CryptoJS.AES.decrypt(s, "heuristiclab").toString(CryptoJS.enc.Utf8);
};