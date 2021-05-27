'use strict';

let drivers = [];   //all drivers
let selStop = null;  //selected stop/row
let selDriver = null;  //current driver
let cachedDriver = null;  //driver selected prior to impersonation
let selRunNumber = null;   //current run #
let isImpersonating = false;

const impersonationPrefix = 'As';

let statuses = [];   //all statuses

const spaces = '&nbsp;&nbsp;&nbsp;&nbsp;'
const minDivHeight = 120;

let signaturePad = null;
let stops = [];


$(document).ready(function () {
    $.noConflict();

    if (window.NodeList && !NodeList.prototype.forEach) {
        NodeList.prototype.forEach = Array.prototype.forEach;
    }

    resizeLogo();

    if (!isAdmin) {
        // load for the run number of the current User
        loadStops(currentRunNumber);
        $('#divAdmin').hide();
        selDriver =
        {
            FullName: driverName,
            CurrentRun: {
                Number: currentRunNumber,
                TruckNumber: truckNumber,
                Note: currentRunNote,
                Signature: null
            }
        };
        loadRunDetail();
        loadNote();
        refreshRun();


    }
    else {
        loadLocations();
        $('#divAdmin').show();
    }

    $('#divStopImpersonation').hide();
    if (isTablet()) {
        $('#MainDiv').css({
            overflowY: 'scroll'
        });
    }


    let canvas = $('#signature');
    signaturePad = (canvas == 0) ? null : new SignaturePad(canvas[0]);

    $('#btnRefresh').click(function () {
        $('form:first').submit();
    });

    $('#ddlLocation').change(function () {
        clearError();
        preservedDriver = null;
        preservedLocation = null;
        preservedSequence = null;
        selRunNumber = null;
        selDriver = null;
        selStop = null;

        $('#hdnLocation').val($('#ddlLocation').val());

        loadDrivers();

        refreshRun();
    });


    //load stops @ driver change
    $('#ddlDriver').change(function () {
        clearError();
        preservedLocation = null;
        preservedDriver = null;
        preservedSequence = null;
        selRunNumber = $(this).val();


        $('#hdnDriver').val($('#ddlDriver').val());

        loadStops(selRunNumber);

        //let grid = $("#grid").data("kendoGrid");

        //grid.refresh();

        let driverList = drivers.filter(function (x) { return x.CurrentRun != null && x.CurrentRun.Number == selRunNumber; });

        if (driverList.length > 0) {
            selDriver = driverList[0];
            loadRunDetail();
            clearSignature();
            refreshRun();
        }

      
    });

    $(window).on("orientationchange", function (event) {
        timedRefresh(5);
    });

    $('#btnDone').click(function () {
        clearSignature();
        $('#modSignature').show();
    });

    $('#btnCloseSignature').click(function () {       
        $('#modSignature').hide();
    });

    $('#btnClearSignature').on('click', function () {
        clearSignature();
    });

    $('#btnCancelSignature').click(function () {
        $('#modSignature').hide();
    });

    $('#btnSaveSignature').click(function () {
        if (signaturePad != null && (signaturePad.isEmpty())) {
            alert('Signature not detected');
            return;
        }

        $('#modSignature').hide();

        updateRun();
    });


    $('#ddlStatus').change(function () {
        $('#btnSaveStop').prop('disabled', false).addClass('btn-primary');
    });
   
    $('#btnSaveStop').click(function () {
        if (!saveStop()) {
            return;
        }

        $('#modStop').hide();
    });

    $('#btnCloseStop').click(function () {
        $('#modStop').hide();
        clearSelectedRow();
    });

    // Edit Note
    $('#refEditNote').click(function () {
        let note = $('#txtNoteRO').val();
        $('#txtNote').val(note);
        $('#modNotes').show();
        $('#btnSaveNotes').attr('disabled', 'disabled');
    });

    $('#txtNote').keyup(function () {
        let enabled = ($('#txtNote').val() != $('#txtNoteRO').val());
        if (enabled) {
            $('#btnSaveNotes').removeAttr('disabled');
        }
        else {
            $('#btnSaveNotes').attr('disabled', 'disabled');
        }
    });


    $('#btnCloseNotes').click(function () {
        $('#modNotes').hide();
    });

    $('#btnCloseStop').click(function () {
        $('#modStop').hide();
    });

    $('#btnClose').click(function () {
        $('#modStop').hide();
    });

   $('#btnSaveNotes').click(function () {
        if (!(selDriver == null || selDriver.CurrentRun == null)) {
            selDriver.CurrentRun.Note = $('#txtNote').val();
            $('#txtNoteRO').val(selDriver.CurrentRun.Note);
        }

       updateRun();

       $('#modNotes').hide();
   });

    $('#btnPopupImpersonate').click(function () {

        loadImpersonationList();
        $('#modImpersonate').show();
    });

    $('#btnImpClose').click(function () {
        $('#modImpersonate').hide();
    });

    $('#btnCloseImpHeader').click(function () {
        $('#modImpersonate').hide();
    });

    $('#btnImpersonate').click(function () {
        doImpersonate();
        $('#modImpersonate').hide();       
    });

    $('#btnStopImpersonation').click(function () {
        stopImpersonation();
    });
});