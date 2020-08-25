/** 
* 时间对象的格式化 
*/
Date.prototype.format = function (format) {
    if (format == null)
        return "";
    /* 
    * format="yyyy-MM-dd hh:mm:ss"; 
    */
    var o = {
        "M+": this.getMonth() + 1,
        "d+": this.getDate(),
        "h+": this.getHours(),
        "m+": this.getMinutes(),
        "s+": this.getSeconds(),
        "q+": Math.floor((this.getMonth() + 3) / 3),
        "S": this.getMilliseconds()
    }

    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }

    for (var k in o) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1
            ? o[k]
            : ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return format;
}

/*---------------------------------------------------
* 字符串转成日期类型
* 格式 MM/dd/YYYY MM-dd-YYYY YYYY-MM-ddT00:00:00 YYYY/MM/ddT00:00:00
---------------------------------------------------*/
function parseDate(dateStr) {
    var year, month, day, hour, minute, second;

    if (dateStr.length == 10) {   //MM/dd/YYYY MM-dd-YYYY
        year = dateStr.substr(6, 4);
        month = dateStr.substr(0, 2);
        day = dateStr.substr(3, 2);
        if (year == 1)
            return null;
        else
            return new Date(year, month - 1, day);
    }
    else if (dateStr.length >= 19) {   //YYYY-MM-ddT00:00:00 YYYY/MM/ddT00:00:00
        year = dateStr.substr(0, 4);
        month = dateStr.substr(5, 2);
        day = dateStr.substr(8, 2);
        hour = dateStr.substr(11, 2);
        minute = dateStr.substr(14, 2);
        second = dateStr.substr(18, 2);

        if (year == 1)
            return null;
        else {
            if (hour == 0 && minute == 0 && second == 0)
                return new Date(year, month - 1, day);
            else
                return new Date(year, month - 1, day, hour, minute, second);
        }
    }
    else {
        alert(dateStr + "不是有效的日期格式!");
        return null;
    }
}
function parseAndFormatDate(dateStr) {
    var dValue = parseDate(dateStr);

    if (dValue != null) {
        return dValue.format("yyyy-MM-dd");
    }
    else {
        return "";
    }
}

function parseAndFormatJsonDateTime(dateStr) {
    var dValue = parseJsonDate(dateStr);

    if (dValue != null)
        return dValue.format("yyyy-MM-dd hh:mm") == '1-01-01 00:00' ? "" : dValue.format("yyyy-MM-dd hh:mm");
    else
        return "";
}

function parseJsonDate(dateStr) {
    return new Date(parseInt(dateStr.substr(6)));
}

function parseAndFormatJsonDate(dateStr) {
    var dValue = parseJsonDate(dateStr);

    if (dValue != null) {
        return dValue.format("yyyy-MM-dd") == "1-01-01" ? "" : dValue.format("yyyy-MM-dd");
    }
    else {
        return "";
    }
}

function parseAndFormatJsonMonth(dateStr) {
    var dValue = parseJsonDate(dateStr);

    if (dValue != null) {
        return dValue.format("yyyy-MM") == "1-01" ? "" : dValue.format("yyyy-MM");
    }
    else {
        return "";
    }
}

function CheckDatePicker(elmentId, elementName)
{
    var dataStr = $("#" + elmentId).val();
    var msgStr = "";
    if (dataStr == undefined) return msgStr;
    if (dataStr == "")
    {
        msgStr = elementName + "不能为空";
        return msgStr;
    }

    var reg = /^(\d{4})-(\d{2})-(\d{2})$/;
    if (!reg.test(dataStr)) {
        msgStr = elementName + "格式不正确";
        return msgStr;
    }

    if (new Date(dataStr).format("yyyy-MM-dd") != dataStr)
    {
        msgStr = elementName + "格式不正确";
        return msgStr;
    }

    return msgStr;
}

/*---------------------------------------------------
* 日期计算
---------------------------------------------------*/
Date.prototype.dateAdd = function (interval, number) {
    var d = this;
    var k = { 'y': 'FullYear', 'q': 'Month', 'm': 'Month', 'w': 'Date', 'd': 'Date', 'h': 'Hours', 'n': 'Minutes', 's': 'Seconds', 'ms': 'MilliSeconds' };
    var n = { 'q': 3, 'w': 7 };
    eval('d.set' + k[interval] + '(d.get' + k[interval] + '()+' + ((n[interval] || 1) * number) + ')');
    return d;
}

/*---------------------------------------------------
* 比较日期差 dtEnd 格式为日期型或者 有效日期格式字符串
---------------------------------------------------*/
Date.prototype.dateDiff = function (interval, dtEnd) {
    var dtStart = this;
    if (typeof dtEnd == 'string')//如果是字符串转换为日期型
    {
        dtEnd = parseDate(dtEnd);
    }
    switch (interval) {
        case 's': return parseInt((dtEnd - dtStart) / 1000);
        case 'n': return parseInt((dtEnd - dtStart) / 60000);
        case 'h': return parseInt((dtEnd - dtStart) / 3600000);
        case 'd': {
            dtStart = new Date(dtStart.getFullYear(), dtStart.getMonth() + 1, dtStart.getDate());
            dtEnd = new Date(dtEnd.getFullYear(), dtEnd.getMonth() + 1, dtEnd.getDate());
            return parseInt((dtEnd - dtStart) / 86400000);
        }
        case 'w': {
            dtStart = new Date(dtStart.getFullYear(), dtStart.getMonth() + 1, dtStart.getDate());
            dtEnd = new Date(dtEnd.getFullYear(), dtEnd.getMonth() + 1, dtEnd.getDate());
            return parseInt((dtEnd - dtStart) / 86400000 / 7);
        }
        case 'm': return (dtEnd.getMonth() + 1) + ((dtEnd.getFullYear() - dtStart.getFullYear()) * 12) - (dtStart.getMonth() + 1);
        case 'y': return dtEnd.getFullYear() - dtStart.getFullYear();
    }
}
