
function onkeyupOnlyNum(element) {
    element.value = element.value.replace(/[^\d]/g, '');
}

function getRadioChecked(name) {
    return $.trim($("input[name='" + name + "']:checked").val());
}

function setRadioChecked(name, value) {
    var radios = document.getElementsByName(name);
    for (i = 0; i < radios.length; i++) {
        if (value == radios[i].value) {
            radios[i].checked = true;
            break;
        }
        else
            radios[i].checked = false;
    }
}

function getFileSizeKB(fileElementId) {
    if ($("#" + fileElementId).val() == "")
        return 0;

    var fileSize = 0;
    //for IE
    if (/msie/.test(navigator.userAgent.toLowerCase())) {
        //before making an object of ActiveXObject,
        //please make sure ActiveX is enabled in your IE browser
        var objFSO = new ActiveXObject("Scripting.FileSystemObject");
        var filePath = $("#" + fileElementId)[0].value;
        var objFile = objFSO.getFile(filePath);
        var fileSize = objFile.size; //size in kb
        fileSize = fileSize / 1048576; //size in mb
    }
        //for FF, Safari, Opeara and Others
    else {
        fileSize = $("#" + fileElementId)[0].files[0].size //size in kb
        fileSize = fileSize / 1048; //size in kb
    }
    return fileSize;
}

//Get File Resource
function PreviewUploadFile(objectTypeId, objectId, fileType, fileDesc, fieldElementId, callback) {
    if ($("#file" + fieldElementId)[0].files.length == 0) return;
    if (getFileSizeKB("file" + fieldElementId) != 0) {
        document.getElementById(fieldElementId).value = document.getElementById("file" + fieldElementId).value;
        if ($("#" + fieldElementId).val() == "") $("#" + fieldElementId + "Path").val("");
    }

    TransferFiles(fieldElementId, objectTypeId, objectId, fileType, fileDesc, callback);
}

function TransferFiles(fieldElementId, objectTypeId, objectId, fileType, fileDesc, callback) {
    var files=$("#file" + fieldElementId)[0].files
    var result=false
    $.each(files, function (index, file) {
        if (file.size > 10000000) {
            result=true
            return
        }
    })
    if(result){
        $("#" + fieldElementId).val("")
        jAlert("所选文件过大，请选择小于10M的文件", "警告", function () { })
        $.ajaxSetup({ async: true });
        return
    }
    SetPageWaiting(true)
    var id=files.length>1?0:$("#" + fieldElementId + "ID").val()
    $.each(files, function (index, file) {
        var reader = new FileReader();
        reader.onload = function (evt) {
            jAlert("<progress max='100' value='0' id='pg'></progress><span id='curr'></span>%", "上传", function () { })
            $("input:button").prop("disabled", true)
            $("#pg").val(0)
            $("#curr").text(0)
            var intervalID = setInterval(function (e) {
                var value = $("#pg").val()
                if (value <= 89) {
                    $("#pg").val(value + 1);
                    $("#curr").text($("#pg").val())
                }
                else clearInterval(intervalID);
            }, 10);
            $.ajaxSetup({ async: false });
            $.post(getRootPath() + '/UploadFile/UploadFile', {
                ObjectTypeId: objectTypeId,
                ObjectID: objectId,
                FileType: fileType,
                FileName: file.name,
                FileDesc: fileDesc,
                ID: id,
                FileContent: evt.target.result.split(',')[1]
            }, function (response) {
                console.log(response.ResultCode)
                if (response.ResultCode != "00") {
                    processResponseError(response.ResultCode, response.ResultMessage);
                    $("#curr").text("失败")
                    $("input:button").prop("disabled", false)
                } else {
                    callback(fieldElementId, response.Data, file.name);
                    var intervalID = setInterval(function (e) {
                        var value = $("#pg").val()
                        if (value <= 99) {
                            $("#pg").val(value + 1);
                            $("#curr").text($("#pg").val())
                        }
                        else {
                            clearInterval(intervalID);
                            $("input:button").prop("disabled", false)
                            $("input:button").click()
                        }
                    }, 30);
                }
            });
            $.ajaxSetup({ async: true });
            if(index==files.length-1)
                SetPageWaiting(false)
        }
        reader.readAsDataURL(file);
    })
}   
                                                                                                                    
function DeleteUploadFile(objectTypeId, fieldElementId, id, callback) {
    jConfirm("请确认是否删除?", "删除", function (result) {                                                             
        if (result) {
            $.post(getRootPath() + '/UploadFile/DeleteUploadFile', {
                objectTypeId: objectTypeId,
                id: id
            }, function (response) {
                if (response.ResultCode != "00") {
                    processResponseError(response.ResultCode, response.ResultMessage);
                } else {
                    callback(fieldElementId);
                }
            });
        }
    });
}

function DownloadFile(objectTypeId, id) {
    $.fileDownload(
        getRootPath() + '/UploadFile/DownloadUploadFile',
        {
            failMessageHtml: "下载文件出错，请重试",
            httpMethod: "POST",
            data: {
                objectTypeId: objectTypeId,
                id: id,
            }
        }
    );
}

//Clear Session
function ClearSession() {
    $.post(getRootPath() + '/Home/ClearSession');
}


function SetTotalPage4Pager(currentPage, totalPages, id,callback) {
    if (totalPages > 0) {
        if (currentPage > totalPages)
            currentPage = totalPages;
    }
    else {
        currentPage = 1;
    }
    var $page = $("#pager")
    var $callback;
    try {
        if (typeof PageClick === "function") { 
            $callback = PageClick
        } else { 
            $callback = function temp() { }
        }
    } catch (e) {
        $callback = function temp() { }
    }
    if (id != null)
        $page = $("#" + id)
    if (callback != null)
        $callback = callback

    $page.pager({
        pagenumber: currentPage,
        pagecount: totalPages,
        buttonClickCallback: $callback
    });

    return currentPage;
}

function UpdateSortIcon(thElement, sortObject) {
    if (thElement.id == sortObject.Field) {
        sortObject.Direction = !sortObject.Direction;
    }
    else {
        sortObject.Field = thElement.id;
        sortObject.Direction = true;
    }

    $("th.sortColumn span").remove();
    AppendSortIcon(thElement, sortObject.Direction);
}

function DisplaySortIcon(sortObject) {
    $("th.sortColumn").each(function () {
        $(this).children().remove();
        if (this.id == sortObject.Field) {
            AppendSortIcon(this, sortObject.Direction);
        }
    });
}

function AppendSortIcon(thElement, direction) {
    if (direction == true) {
        $(thElement).append("<span class='sortIcon asc'></span>");
    }
    else {
        $(thElement).append("<span class='sortIcon dsc'></span>");
    }
}

function PrintBody() {
    window.print();
    window.location.reload();
}

function getMinDate(elementId) {
    if ($.trim($("#" + elementId).val()) == "")
        return "1900-01-01";
    else
        return $("#" + elementId).val();
}

function getMaxDate(elementId) {
    if ($.trim($("#" + elementId).val()) == "")
        return "2999-01-01";
    else
        return $("#" + elementId).val();
}

function SetPageWaiting(isWaiting) {
    if (isWaiting) {
        $("button").prop("disabled", true);
        $("body").css("cursor", "progress");
    } else {
        $("button").prop("disabled", false);
        $("body").css("cursor", "default");
    }
}

//functions for chart style and slider
function getChartMaxItems()
{
    return 15;
}
function SelectYears(id, endDate) {
    id = id || 'myYear'
    endDate = endDate || 0
    var myDate = new Date();
    var startYear = myDate.getFullYear() - 10;//起始年份
    var endYear = myDate.getFullYear() + endDate;//结束年份
    var obj = document.getElementById(id);
    obj.options.length = 0;
    for (var i = startYear; i <= endYear; i++) {
        obj.options.add(new Option(i, i));
    }
    obj.options[obj.options.length - 11].selected = 1;
}

function SelectMonths(id) {
    id = id || 'myMonth'
    var month = document.getElementById(id);
    for (var j = 1; j <= 12; j++) {
        month.add(new Option(j, j));
    }
}

function PrintChart(chart,reportName) {
    if (chart == undefined) {
        jAlert("无图片可打印", "错误");
        return;
    }
    setTimeout(function () {
        chart.downloadImage(reportName);
    }, 1500);
}
function CreateChart()
{
    $("#chartDiv").empty();

    chart = new G2.Chart({
        container: 'chartDiv',
        forceFit: true,
        height: 600,
        padding: [30, 120, 120, 90],
        background: {
            fill: "#F8F9FA"
        }
    });

    return chart;
}

function SetChartStyle(barView, yName) {
    barView.axis('value', {
        label: {
            textStyle: {
                endArrow: true,
                fill: '#17A2B8'
            },
        },
        title: {
            textStyle: {
                fill: '#17A2B8'
            },
            offset: 50
        }
    });
    barView.axis('key', {
        label: {
            offset: 5,
            offsetY: 15,
        }
    });
    barView.scale({
        value: {
            alias: yName
        },
    });
}

function BoundSlide2Chart(chart, sliderWidth, itemList, yName, typeID) {
    $("#sliderDiv").empty();

    var maxItems = getChartMaxItems();
    if (maxItems >= itemList.length) {
        chart.source(itemList);
        return chart;
    }

    const ds = new DataSet({
        state: {
            from: itemList[0].key,
            to: itemList[maxItems - 1].key
        }
    })

    const dv = ds.createView();

    dv.source(itemList).transform({
        type: 'filter',
        callback: function (obj) {
            for (var i = 0; i < itemList.length; i++) {
                if (itemList[i].key == ds.state.from)
                    var startIndex = i;
                if (itemList[i].key == ds.state.to)
                    var endIndex = i;
                if (itemList[i].key == obj.key)
                    var index = i;
            }

            return index >= startIndex && index <= endIndex;
        }
    });

    const barView = chart.view()
    barView.source(dv, {});

    chart.interaction('slider', {
        container: 'sliderDiv',
        width: sliderWidth,
        height: 26,
        padding: [20, 90, 90, 90],
        //startRadio: 0,
        //endRadio: (maxItems - 1) / itemList.length,
        startRadio: typeID == 1 ? (itemList.length - maxItems) / itemList.length : 0,// 1代表'@BusinessObjects.Domain.ReportDimension.AcceptanceYear'
        endRadio: typeID == 1 ? 1 : (maxItems - 1) / itemList.length, // 1代表'@BusinessObjects.Domain.ReportDimension.AcceptanceYear'
        data: itemList,
        xAxis: 'key',
        yAxis: yName,
        fillerStyle: {
            fill: '#BDCCED',
            fillOpacity: 0.7,//透明度
        }, // 滑块选中区域的样式

        backgroundStyle: {
            stroke: '#CCD6EC',
            fill: '#CCD6EC',
            fillOpacity: 0.2,
            lineWidth: 2,//边框宽度
        }, // 滑块背景样式
        layout: 'horizontal', // 滑块的布局，'horizontal' 或者 'vertical'
        onChange: function (text) {
            ds.setState('from', text.startText);
            ds.setState('to', text.endText);
            setTimeout(function () {
                chart.render();
            });
        }
    });

    return barView;
}
//end chart

function getVersion() {
    return "20200421";
}

function CheckFileName4Preview(fileName) {
    return (/\.(gif|jpe?g|tiff|png|webp|bmp)$/i).test(fileName);
}

function autoCompleteDepartments(info) {
    $("#Department").autocomplete({
        minLength: 1,
        delay: 200,//ms
        source: function (req, add) {
            $.ajax({
                type: "get",
                async: false,
                secureuri: false,
                url: getRootPath() + '/Department/QueryDepartment4AutoComplete',
                data: {
                    inputText: $.trim(req.term),
                },
                dataType: 'json',
                success: function (response) {
                    response = JSON.parse(JSON.stringify(response));
                    if (response.ResultCode != "00") {
                        processResponseError(response.ResultCode, response.ResultMessage);
                    } else {
                        info.Department.ID = -1;//clear
                        var infos = response.Data;
                        add(infos);
                    }
                },
                error: function () { jAlert("连接服务器出错", "错误"); }
            });
        },
        focus: function (event, ui) {
            info.Department = ui.item;
            info.Department.Name = ui.item.Description;
            return false;
        },
        select: function (event, ui) {
            info.Department = ui.item;
            info.Department.Name = ui.item.Description;
            return false;
        }
    })
    .data("ui-autocomplete")._renderItem = function (ul, item) {
        return $("<li>")
          .append("<a>" + item.Description + "-" + item.Pinyin + "</a>")
          .appendTo(ul);
    };
}