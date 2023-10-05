//connection types
let siteInfotable = "siteInfotable";
$.extend(fotfmanager.client, {
    updateSiteInfo: async (updateData) => { Promise.all([updateSiteInfoDataTable(updateData, siteInfotable)]); }
});
async function init_SiteInfo(data) {
    try {
        Promise.all([createSiteInfoDataTable(siteInfotable)]);
        if (data) {
            Promise.all([updateSiteInfoDataTable(formatSiteInfodata(data), siteInfotable)]);
        }
    } catch (e) {
        throw new Error(e.toString());
    }
}

async function createSiteInfoDataTable(table) {
    let arrayColums = [{
        "KEY_NAME": "",
        "VALUE": "",
        "Action": ""
    }]
    let columns = [];
    let tempc = {};
    $.each(arrayColums[0], function (key) {
        tempc = {};
        if (/KEY_NAME/i.test(key)) {
            tempc = {
                "title": 'Name',
                "width": "30%",
                "mDataProp": key
            }
        }
        if (/VALUE/i.test(key)) {
            tempc = {
                "title": "Description",
                "width": "50%",
                "mDataProp": key
            }
        }
        if (/Action/i.test(key)) {
            tempc = {
                "title": "Action",
                "width": "10%",
                "mDataProp": key,
                "mRender": function () {
                    return '<button class="btn btn-light btn-sm mx-1 pi-iconEdit editsiteInfo" data-toggle="modal" name="editsiteInfo"></button>';
                }
            }
        }
        columns.push(tempc);

    });
   $('#' + table).DataTable({
        dom: 'Bfrtip',
        bFilter: false,
        bdeferRender: true,
        bpaging: false,
        bPaginate: false,
        autoWidth: false,
        bInfo: false,
        destroy: true,
        language: {
            emptyTable: "No Data Available"
        },
        aoColumns: columns,
        columnDefs: [
        ],
        sorting: [[0, "asc"]]

    })

    // Edit record
    $('#' + table + ' tbody').on('click', 'button', function () {
        let td = $(this);
        let table = $(td).closest('table');
        let row = $(table).DataTable().row(td.closest('tr'));
        if (/editsiteInfo/ig.test(this.name)) {
            Edit_siteInfo(row.data());
        }
    });
}
async function loadSiteInfoDataTable(data, table) {
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows.add(data).draw();
    }
}
async function updateSiteInfoDataTable(newdata, table) {
    let loadnew = true;
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
            loadnew = false;
            $('#' + table).DataTable().row(node).data(element).draw().invalidate();

        })
        if (loadnew) {
            loadSiteInfoDataTable(newdata, table);
        }
    }
}
function Edit_siteInfo(Data) {
    $('.valuediv').css("display", "block");
    $('.timezonediv').css("display", "none");
    $('.value_row_toggle').css("display", "none");
    $('#appsettingvaluemodalHeader').text('Edit ' + Data.KEY_NAME + ' Info');
    $('input[id=modalKeyID]').val(Data.KEY_NAME);
    $('input[id=modalValueID]').val(Data.VALUE);



    $('button[id=appsettingvalue]').off().on('click', function () {
        $('button[id=appsettingvalue]').prop('disabled', true);
        let jsonObject = {};

        jsonObject[Data.KEY_NAME] = Get_value(Data);

        if (!$.isEmptyObject(jsonObject)) {
            fotfmanager.server.editSiteInfo(JSON.stringify(jsonObject)).done(function (data) {

                $('span[id=error_appsettingvalue]').text("Data has been updated");
                setTimeout(function () {
                    $("#AppSetting_value_Modal").modal('hide');
                    updateSiteInfoDataTable(formatSiteInfodata(data), siteInfotable);
                }, 800);
            });
        }
    });
    $('#AppSetting_value_Modal').modal();
}
function formatSiteInfodata(result) {
    let reformatdata = [];
    try {
        for (let key in result) {
            if (result.hasOwnProperty(key)) {
                let temp = {
                    "KEY_NAME": "",
                    "VALUE": ""
                };
                temp['KEY_NAME'] = key;
                temp['VALUE'] = result[key];
                reformatdata.push(temp);
            }
        }

    } catch (e) {
        throw new Error(e.toString());
    }

    return reformatdata;
}