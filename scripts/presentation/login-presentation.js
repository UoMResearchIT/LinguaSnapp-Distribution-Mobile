// Class:   
//          LoginPresentation
//          Presentation functionality for the login screen
// Constructor parameters:
//          db: The BrodySoft PhoneGap SQLlite plugin database reference or WebSQL database reference.
var LoginPresentation = function (db) {


    this.linguaDB = db;

    // registerEvents
    // Register any events associated with the login form
    this.registerEvents = function () {

        var self = this;

        $(document).on("pageinit", "#login", function () {
            var form = $("#frmLogin");
            form.validate({
                rules: {
                    txtLoginUsercode: {

                        pattern: /[A-Za-z0-9]{5,20}/
                    },

                    txtLoginPassword: {
                        minlength: 5,

                    }
                },
                messages: {
                    txtLoginUsercode: {
                        pattern: "At least 5 characters, letters or numbers only."
                    },
                    txtLoginPassword: {
                        minlength: jQuery.validator.format("At least {0} characters required!"),
                    }
                }

            });

        });



        // Login button clicked.
        $("#btnLogin").on("tap", function (e) {

            var form = $("#frmLogin");
            var isValid = form.valid();


            if (isValid === true) {
                var login = new Login(self.linguaDB);

                // Call the login function
                var promise = login.sendLoginInformation($("#txtLoginUsercode").val(), $("#txtLoginPassword").val());

                promise.done(function () {
                    // If logged on successfully redirect page
                    $(':mobile-pagecontainer').pagecontainer("change", "#main", { changeHash: false });
                });

                promise.always(function () {
                    $.mobile.loading('hide');
                });
            }

            e.stopPropagation();
            e.preventDefault();
        });

        // Stop the default keyboard submit on mobile devices from reloading the page.
        $("#frmLogin").submit(function () {
            e.stopPropagation();
            return false;
        });

    }




}