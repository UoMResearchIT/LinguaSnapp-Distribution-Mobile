// Class:   
//          RegisterPresentation
//          Presentation functionality for the register screen
// Constructor parameters:
//          db: The BrodySoft PhoneGap SQLlite plugin database reference or WebSQL database reference.
var RegisterPresentation = function (db) {


    this.linguaDB = db;

     // registerEvents
    // Register any events associated with the registration form
    this.registerEvents = function () {

        var self = this;

        $(document).on("pageinit", "#register", function () {
            var form = $("#frmRegister");
            form.validate({
                rules: {
                    txtUsercode: {
                        
                        pattern: /[A-Za-z0-9]{5,20}/
                    },

                    txtPassword: {
                        minlength: 5,

                    },

                    txtConfirm: {
                        minlength: 5,
                        equalTo: "#txtPassword",

                    }
                },
                messages: {
                    txtUsercode: {

                       
                        pattern: "At least 5 characters, letters or numbers only."

                    },
                    txtPassword: {

                        minlength: jQuery.validator.format("At least {0} characters required!"),


                    },

                    txtConfirm: {

                        minlength: jQuery.validator.format("At least {0} characters required!"),

                        equalTo: "Your passwords do not match."
                    }
                }

            });

        });



        // Register button clicked.
        $("#btnRegister").on("tap", function (e) {
            
            var form = $("#frmRegister");
            var isValid = form.valid();


            if (isValid === true) {
                var register = new Register(self.linguaDB);

                // Call the registration function
                var promise = register.sendRegisterInformation($("#txtUsercode").val(), $("#txtPassword").val(), $("#emailAddress").val(), $("input:radio[name='mailingList']:checked").val());

                promise.done(function () {
                    // If not registered redirect page
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
        $("#frmRegister").submit(function () {
            e.stopPropagation();
            return false;
        });

    }


   

}