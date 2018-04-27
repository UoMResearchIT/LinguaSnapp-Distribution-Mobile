
function registerEvents(db) {

    var re = new RegisterPresentation(db);
    re.registerEvents();

    var le = new LoginPresentation(db);
    le.registerEvents();

    var he = new HomePresentation(db);
    he.registerEvents();

    var pe1 = new Photo1Presentation(db);
    pe1.registerEvents();

    var pe2 = new Photo2Presentation(db);
    pe2.registerEvents();

    var pe3 = new Photo3Presentation(db);
    pe3.registerEvents();

    var pen = new PendingPresentation(db);
    pen.registerEvents();



    // Clear the history to stop the back button (we only want navigation from the on-screen buttons) 
    $(document).on('pagecontainerchange', function (e, ui) {

        e.stopPropagation();
    });

    // Disable the back button on Android
    document.addEventListener("backbutton", function (e) {
        e.preventDefault();
    }, false);

    // Try to stop the device's keyboard 'done' button submitting the form
    $('input').keypress(function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if ((code == 13) || (code == 10)) {
            $(this).blur();
            return false;
        }
    });


    $.mobile.defaultPageTransition = "none"; // No transitions - don't seem to work on some versions of Android
    $.mobile.defaultDialogTransition = "none"; // No transitions - don't seem to work on some versions of Android
}




function init(db) {

   

    registerEvents(db);

    
    var popDB = new PopulateDB(db);
    var register = new Register(db);

    var promise = popDB.populate();

    promise.done(function () {
        register.checkRegistered();
    });

}

// Initialise the About screen and any app-specific text
function doAboutScreen()
{
    var phoneGapUtils = new PhoneGapUtils();
    var config = new Config();
    phoneGapUtils.addAppText();

    $('#about-para1').html($.i18n('about-para1', phoneGapUtils.getAppText('app-title'), phoneGapUtils.getAppText('app-location')));
    $('#about-para2').html($.i18n('about-para2', phoneGapUtils.getAppText('app-title')));
    $('#about-para3').html($.i18n('about-para3', phoneGapUtils.getAppText('app-title')));
    $('#about-para4').html($.i18n('about-para4', phoneGapUtils.getAppText('app-title')));
    $('#about-para5').html($.i18n('about-para5', phoneGapUtils.getAppText('app-title')));

    $('#about-maplocation').html("<a href='#' onclick=\"window.open('" + config.mapLocation  + "', '_system')\">" + config.mapLocation +"</a>.")

    $('#about-credits').html($.i18n('about-credits', phoneGapUtils.getAppText('app-title')));

    $('#menu-about').html($.i18n('menu-about', phoneGapUtils.getAppText('app-title')));
}

// Localise the validator messages
function doValidatorMessages(lang)
{
    if (lang === "de") {
        $.extend($.validator.messages, {
            required: "Dieses Feld ist ein Pflichtfeld.",
            maxlength: $.validator.format("Geben Sie bitte maximal {0} Zeichen ein.")
        });
    }
}

// Device is ready (app loaded)
function onDeviceReady() {



    // iOS Statusbar
    if (device.platform.toLowerCase === "ios") {
        StatusBar.overlaysWebView(false);
        StatusBar.styleDefault();
    }

    var db = window.sqlitePlugin.openDatabase({ name: "LinguaSnapp.db", location: "default" });
    init(db);

    $.i18n().load({
        'en': './scripts/i18n/en.json',
        'de': './scripts/i18n/de.json'
    }).done(function () {


        navigator.globalization.getPreferredLanguage(
            function (language) {
                var lang = language.value.split("-")[0];
                
                $.i18n({
                    locale: lang
                });

                $('body').i18n();
                doAboutScreen();
                doValidatorMessages(lang);
            },
            function () {
                // if there is an error getting the preferred language then default to English
                $.i18n({
                    locale: 'en'
                });

                $('body').i18n();
                doAboutScreen();
            }
            );
    })
}


$(document).ready(function () {


    // Wait for app to load
    document.addEventListener("deviceready", onDeviceReady, false);


    if ($("#mobile").val() === "no") {
        var db = window.openDatabase("LinguaSnapp.db", "1.0", "LinguaSnapp", 1024 * 1024);
        init(db);

        // if you are testing on a browser then you can manually change the language in the 'locale' setting below.
        $.i18n().load({
            'en': './scripts/i18n/en.json',
            'de': './scripts/i18n/de.json'
        }).done(function () {
            $.i18n({
                locale: 'de'
            });

            $('body').i18n();
            doAboutScreen();
            doValidatorMessages("de");
            
        })

       

    }
});