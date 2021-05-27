'use strict';

let checkedIds = {};

let drivers = [];
let isCustomerEditMode = false;

let selCustomer = null;
let selDriver = null;
let selRunNumber = null;

const spaces = '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' 


$(document).ready(function () {
    $.noConflict();  

    if (!isAdmin) {
        // load for the run number of the current User
        loadCustomers(currentRunNumber);

        $('#divDropdowns').hide();
    }
    else {
        loadLocations();
        $('#divDropdowns').show();
   }

    $('#ddlLocation').change(function () {
        loadDrivers();
    });

    $('#ddlDriver').change(function () {
        selRunNumber = $(this).val();
        loadCustomers(selRunNumber);

        let grid = $("#grid").data("kendoGrid");

        grid.refresh();

        let driver = drivers.find(d => d.CurrentRun.Number == selRunNumber);
        if (driver != null) {
            selDriver = driver;
            loadHeader(selDriver);
        }
    });

});

function loadCustomers(runID) {
    if (runID == null) {
        return;
    }

    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: baseUrl + 'api/customer?rid=' + runID,
        success: function (data) {
            let grid = $("#grid").kendoGrid({

                dataSource: {
                    data: data,
                    schema: {
                        model: {
                            fields: {
                                ID: { type: 'number'},
                                Name: { type: "string" },
                                Address: { type: "string" },
                                City: { type: "string" }
                            }
                        }
                    },
                    pageSize: 20
                },
                selectable: true,
                scrollable: true,
                sortable: true,
                filterable: false,
                groupable: false,
                pageable: {
                    input: true,
                    numeric: true 
                },
                persistSelection: false,
                title: "[Stops]",
                columns: [                  
                   
                    {
                        field: "Name",
                        attributes: { class: "text-left" },
                        template: '<div><a href="\\#" id="custNameLink">#= Name#</a></div>',
                        headerTemplate: 'Customer Name <span class="k-i-kpi"></span>'
                    },

                    {
                        field: 'FullAddress',
                //        title: "Address",                        
                        attributes: { class: "text-left;" },
                        template: '<div><a href = "\\#" id = "custAddressLink" >#= FullAddress#</a ></div > ',
                        headerTemplate: 'Address <span class="k-i-kpi"></span>'
                    },

                     {
                        field: 'ID',
                        width: 85,
                        title: " ",
                        template: function () {
                            const delim = '    ';
                            return '<a id="editLink"><img src="../Images/edit.png" title="Edit" style="height: 20px; width: 20px" id="imgEdit"></img></a>' + delim +
                                '<a id="mapLink"><img src="../Images/Map.png" title="Map" style="height: 20px; width: 20px" id="imgMap"></img></a>';
                        }
                    }
                ]
            });

            let custGrid = grid.data("kendoGrid");


            custGrid.element.on("click", "#custNameLink", viewCustomer);
            custGrid.element.on("click", "#imgEdit", editCustomer);
            custGrid.element.on("click", "#imgMap", showMap);
            custGrid.element.on("click", "#custAddressLink", showMap);


        },
        error: function (response) {
            console.log(response.responseText);

        }
    });

  
    $('#btnClose').click(function () {
        $('#modCustomer').hide();
    });

    $('#custSave').click(function () {
        if (isCustomerEditMode) {
            if (!saveCustomer()) {
                return;
            }
        }
        $('#modCustomer').hide();
    });

    $('#custClose').click(function () {
        $('#modCustomer').hide();
    });

}

function viewCustomer() {
    isCustomerEditMode = false;

    var row = $(this).closest("tr"),
        grid = $("#grid").data("kendoGrid"),
        data = grid.dataSource.options.data,
        dataItem = grid.dataItem(row);

    showCustomerPopup( grid, data, dataItem);

    $("#divButtons").hide();

}

function editCustomer() {
    isCustomerEditMode = true;

    var row = $(this).closest("tr"),
        grid = $("#grid").data("kendoGrid"),
        data = grid.dataSource.options.data,
        dataItem = grid.dataItem(row);

    $("#divButtons").show();

    showCustomerPopup(grid,  data, dataItem);
}

function showCustomerPopup(grid, data, dataItem) {

    selCustomer = null;

     selCustomer = data.find(c => c.ID === dataItem.ID);

    if (selCustomer != null) {

        let c_name = selCustomer.Name;
        let c_address = selCustomer.Address;
        let c_fullAddress = selCustomer.FullAddress;

        $('#roCustName').html(c_name);
   //     $('#roCustAddress').html(c_fullAddress);

        $('#rwCustName').val(c_name);
   //     $('#txtCustAddress').val(c_address);

    }

    let modTitle = (isCustomerEditMode) ? 'Edit' : 'View';

    $('#custPopupTitle').html(modTitle);

    $('[id^="val"]').hide();
    if (isCustomerEditMode) {
        $('[id^="ro"]').hide();
        $('[id^="rw"]').show();
    }
    else {
        $('[id^="rw"]').hide();
        $('[id^="ro"]').show();
    }

    $('#modCustomer').show(); 
}

function saveCustomer() {
    if ($("#rwCustName").val() == '') {
        $('#valCustomer').show();
        return false;
    }

    selCustomer.Name = $("#rwCustName").val();

    updateCustomer();

    return true;
}

function showMap(grid, data, dataitem) {

    let mapUrl = null;

    var row = $(this).closest("tr"),
        grid = $("#grid").data("kendoGrid"),
        data = grid.dataSource.options.data,
        dataItem = grid.dataItem(row);

    //selCustomer = null;

    selCustomer = data.find(c => c.ID === dataItem.ID);

    if (selCustomer != null) {
        mapUrl = selCustomer.MapUrl;
    }


    window.open(mapUrl);

}


function updateCustomer() {
    $.ajax({
        type: 'PUT',
        dataType: 'json',
        data: selCustomer,
        url: baseUrl + 'api/customer',
        success: function (data) {
            loadCustomers(selRunNumber);
            let grid = $("#grid").data("kendoGrid");

            grid.sync();
        },
        error: function (response) {
            console.log(response.responseText);

        }
    });
}

function loadLocations() {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: baseUrl + 'api/location',
        success: function (data) {
            $('#ddlLocation').find('option').remove();
            for (let i = 0; i < data.length; i++) {
                let code = data[i].Code;
                $('#ddlLocation').append('<option value="' + code + '">' + code + '</option>');
            }

            $('#ddlLocation').val(userLocation);
            loadDrivers();
        },
        error: function (response) {
            console.log(response.responseText);

        }
    });
}

function loadDrivers() {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: baseUrl + 'api/driver?locCode=' + $('#ddlLocation').val(),
        success: function (data) {
            drivers = data;
            $('#ddlDriver').find('option').remove();
            for (let i = 0; i < data.length; i++) {
                let driver = data[i];
                let runNumber = (driver.CurrentRun == null) ? '' : driver.CurrentRun.Number;
                $('#ddlDriver').append('<option value="' + runNumber + '">' + driver.FullName + '</option>');
            }

            if (data.length > 0) {
                selDriver = data[0];

                selRunNumber = (selDriver.CurrentRun == null) ? '' : selDriver.CurrentRun.Number;
                $('#ddlDriver').val(selRunNumber);

                loadHeader(selDriver);
                loadCustomers(selRunNumber);
            }
            else {
                $('#grid').data("kendoGrid").dataSource.data([]);

                let grid = $("#grid").data("kendoGrid");

                grid.refresh();
                $("#divCustTitle").html('');
          }
        },
        error: function (response) {
            console.log(response.responseText);

        }
    });
}

function loadHeader(selDriver) {

    let custTitle = '<h3>' +
        selDriver.FullName +
        spaces +
        'Route: ' + $('#ddlDriver').val() +
        spaces +
        'Truck: ' + selDriver.TruckNumber +
        spaces +
        '</h3 >';

    $("#divCustTitle").html(custTitle);
}


