
// Class:   
//          populateDB
//          Populates the database, if it is not already available
// Constructor parameters:
//          db: The BrodySoft PhoneGap SQLlite plugin database reference or WebSQL database reference.


var PopulateDB = function(db)
    {

    
    this.linguaDB = db;
    this.phoneGapUtils = new PhoneGapUtils();

    this.appVersion = "1.0.0";

    this.configDB = new ConfigDB(db);

    

    this.descriptorData = {
        
        // descriptors fields:
        // id (integer): internal SQLite primary key
        // code (text): external unique idenfier (2 characters)
        // description (text): display label for the field
        // other (integer): 1 = user may enter an 'other' text, 0 = they can't
        // multi (integer): 1 = user may choose more than one entry, 0 = they can't
        // if_yes_display (integer): <num > 0> = this is a yes/no switch, and when set to yes display the question with an ID of num (else hide that question)
        "descriptors": [
            { "id": 1, "code": "LA", "description": "Language", "other": 1, "multi": 0, "if_yes_display": 0 },
            { "id": 2, "code": "AL", "description": "Alphabet", "other": 1, "multi": 0, "if_yes_display": 0 },
            { "id": 3, "code": "PO", "description": "Position", "other": 0, "multi": 0, "if_yes_display": 0 },
            { "id": 4, "code": "SI", "description": "Sign Type", "other": 0, "multi": 0, "if_yes_display": 0 },
            { "id": 5, "code": "OU", "description": "Outlet", "other": 1, "multi": 0, "if_yes_display": 0 },
            { "id": 6, "code": "DE", "description": "Design", "other": 0, "multi": 1, "if_yes_display": 0 },
            { "id": 7, "code": "CO", "description": "Content", "other": 1, "multi": 1, "if_yes_display": 0 },
            { "id": 8, "code": "SE", "description": "Sector", "other": 0, "multi": 0, "if_yes_display": 0 },
            { "id": 9, "code": "AU", "description": "Audience", "other": 0, "multi": 0, "if_yes_display": 0 },
            { "id": 10, "code": "PU", "description": "Purpose", "other": 0, "multi": 1, "if_yes_display": 0 },
            { "id": 11, "code": "FU", "description": "Function", "other": 0, "multi": 1, "if_yes_display": 0 },
            { "id": 12, "code": "AR", "description": "Arrangement", "other": 0, "multi": 1, "if_yes_display": 0 },
            { "id": 13, "code": "ON", "description": "One language dominant?", "other": 0, "multi": 0, "if_yes_display": 14 },
            { "id": 14, "code": "DO", "description": "Dominance", "other": 0, "multi": 1, "if_yes_display": 0 },
            { "id": 15, "code": "NA", "description": "Num. Alphabets", "other": 0, "multi": 0, "if_yes_display": 0 },
            { "id": 16, "code": "NL", "description": "Num. Languages", "other": 0, "multi": 0, "if_yes_display": 0 }
        ],


        // descriptor_values fields:
        // id (integer) internal SQLite primary key
        // descriptor_id (integer): Link to descriptors table. The descriptor for which this is a value
        // value_code (text): external unique identifier (5 characters)
        // value_name (text): display value
        // sequence_no (integer): order in which it appears on the drop-down list. Equal numbers will sorted in alphabetical order of value_name.
        "descriptor_values": [
            // Languages
            { "id": 101, "descriptor_id": 1, "value_code": "LA001", "value_name": "English", "sequence_no": 1, "active": 1 },
            { "id": 102, "descriptor_id": 1, "value_code": "LA002", "value_name": "Urdu", "sequence_no": 2, "active": 1 },
            { "id": 103, "descriptor_id": 1, "value_code": "LA003", "value_name": "Arabic", "sequence_no": 3, "active": 1 },
            { "id": 104, "descriptor_id": 1, "value_code": "LA004", "value_name": "Chinese", "sequence_no": 4, "active": 1 },
            { "id": 105, "descriptor_id": 1, "value_code": "LA005", "value_name": "Amharic", "sequence_no": 5, "active": 1 },
            { "id": 106, "descriptor_id": 1, "value_code": "LA006", "value_name": "Bengali", "sequence_no": 6, "active": 1 },
            { "id": 107, "descriptor_id": 1, "value_code": "LA007", "value_name": "Bosnian/Croatian", "sequence_no": 7, "active": 1 },
            { "id": 108, "descriptor_id": 1, "value_code": "LA008", "value_name": "French", "sequence_no": 8, "active": 1 },
            { "id": 109, "descriptor_id": 1, "value_code": "LA009", "value_name": "Gujarati", "sequence_no": 9, "active": 1 },
            { "id": 110, "descriptor_id": 1, "value_code": "LA010", "value_name": "Greek", "sequence_no": 10, "active": 1 },
            { "id": 111, "descriptor_id": 1, "value_code": "LA011", "value_name": "Hebrew", "sequence_no": 11, "active": 1 },
            { "id": 112, "descriptor_id": 1, "value_code": "LA012", "value_name": "Hindi", "sequence_no": 12, "active": 1 },
            { "id": 113, "descriptor_id": 1, "value_code": "LA013", "value_name": "Italian", "sequence_no": 13, "active": 1 },
            { "id": 114, "descriptor_id": 1, "value_code": "LA014", "value_name": "Japanese", "sequence_no": 14, "active": 1 },
            { "id": 115, "descriptor_id": 1, "value_code": "LA015", "value_name": "Korean", "sequence_no": 15, "active": 1 },
            { "id": 116, "descriptor_id": 1, "value_code": "LA016", "value_name": "Kurdish (Kurmanji)", "sequence_no": 16, "active": 1 },
            { "id": 117, "descriptor_id": 1, "value_code": "LA017", "value_name": "Kurdish (Sorani)", "sequence_no": 17, "active": 1 },
            { "id": 118, "descriptor_id": 1, "value_code": "LA018", "value_name": "Latvian", "sequence_no": 18, "active": 1 },
            { "id": 119, "descriptor_id": 1, "value_code": "LA019", "value_name": "Lithuanian", "sequence_no": 19, "active": 1 },
            { "id": 120, "descriptor_id": 1, "value_code": "LA020", "value_name": "Malay", "sequence_no": 20, "active": 1 },
            { "id": 121, "descriptor_id": 1, "value_code": "LA021", "value_name": "Malayalam", "sequence_no": 21, "active": 1 },
            { "id": 122, "descriptor_id": 1, "value_code": "LA022", "value_name": "Pashto", "sequence_no": 22, "active": 1 },
            { "id": 123, "descriptor_id": 1, "value_code": "LA023", "value_name": "Persian (Farsi)", "sequence_no": 23, "active": 1 },
            { "id": 124, "descriptor_id": 1, "value_code": "LA024", "value_name": "Polish", "sequence_no": 24, "active": 1 },
            { "id": 125, "descriptor_id": 1, "value_code": "LA025", "value_name": "Punjabi", "sequence_no": 25, "active": 1 },
            { "id": 126, "descriptor_id": 1, "value_code": "LA026", "value_name": "Romanian", "sequence_no": 26, "active": 1 },
            { "id": 127, "descriptor_id": 1, "value_code": "LA027", "value_name": "Russian", "sequence_no": 27, "active": 1 },
            { "id": 128, "descriptor_id": 1, "value_code": "LA028", "value_name": "Somali", "sequence_no": 28, "active": 1 },
            { "id": 129, "descriptor_id": 1, "value_code": "LA029", "value_name": "Spanish", "sequence_no": 29, "active": 1 },
            { "id": 130, "descriptor_id": 1, "value_code": "LA030", "value_name": "Tamil", "sequence_no": 30, "active": 1 },
            { "id": 131, "descriptor_id": 1, "value_code": "LA031", "value_name": "Tigrinya", "sequence_no": 31, "active": 1 },
            { "id": 132, "descriptor_id": 1, "value_code": "LA032", "value_name": "Ukrainian", "sequence_no": 32, "active": 1 },
            { "id": 133, "descriptor_id": 1, "value_code": "LA033", "value_name": "Vietnamese", "sequence_no": 33, "active": 1 },
            { "id": 134, "descriptor_id": 1, "value_code": "LA034", "value_name": "Turkish", "sequence_no": 34, "active": 1 },
            { "id": 135, "descriptor_id": 1, "value_code": "LA035", "value_name": "Yiddish", "sequence_no": 35, "active": 1 },
            { "id": 136, "descriptor_id": 1, "value_code": "LA036", "value_name": "Welsh", "sequence_no": 36, "active": 1 },
            { "id": 137, "descriptor_id": 1, "value_code": "LA037", "value_name": "Scottish Gallic", "sequence_no": 37, "active": 1 },
            { "id": 138, "descriptor_id": 1, "value_code": "LA038", "value_name": "Gaelic (Irish)", "sequence_no": 38, "active": 1 },
            { "id": 139, "descriptor_id": 1, "value_code": "LA039", "value_name": "Manx", "sequence_no": 39, "active": 1 },
            { "id": 140, "descriptor_id": 1, "value_code": "LA040", "value_name": "Cornish", "sequence_no": 40, "active": 1 },
            { "id": 141, "descriptor_id": 1, "value_code": "LA041", "value_name": "Armenian", "sequence_no": 41, "active": 1 },
            { "id": 142, "descriptor_id": 1, "value_code": "LA042", "value_name": "Bulgarian", "sequence_no": 42, "active": 1 },
            { "id": 143, "descriptor_id": 1, "value_code": "LA043", "value_name": "Czech", "sequence_no": 43, "active": 1 },
            { "id": 144, "descriptor_id": 1, "value_code": "LA044", "value_name": "Dutch", "sequence_no": 44, "active": 1 },
            { "id": 145, "descriptor_id": 1, "value_code": "LA045", "value_name": "Hungarian", "sequence_no": 45, "active": 1 },
            { "id": 146, "descriptor_id": 1, "value_code": "LA046", "value_name": "Mongolian", "sequence_no": 46, "active": 1 },
            { "id": 147, "descriptor_id": 1, "value_code": "LA047", "value_name": "Nepali", "sequence_no": 47, "active": 1 },
            { "id": 148, "descriptor_id": 1, "value_code": "LA048", "value_name": "Portuguese", "sequence_no": 48, "active": 1 },
            { "id": 149, "descriptor_id": 1, "value_code": "LA049", "value_name": "Serbian", "sequence_no": 49, "active": 1 },
            { "id": 150, "descriptor_id": 1, "value_code": "LA050", "value_name": "Sinhala", "sequence_no": 50, "active": 1 },
            { "id": 151, "descriptor_id": 1, "value_code": "LA051", "value_name": "Slovak", "sequence_no": 51, "active": 1 },
            { "id": 152, "descriptor_id": 1, "value_code": "LA052", "value_name": "Slovenian", "sequence_no": 52, "active": 1 },
            { "id": 153, "descriptor_id": 1, "value_code": "LA053", "value_name": "Swahili", "sequence_no": 53, "active": 1 },
            { "id": 154, "descriptor_id": 1, "value_code": "LA054", "value_name": "Thai", "sequence_no": 54, "active": 1 },
            { "id": 155, "descriptor_id": 1, "value_code": "LA055", "value_name": "Tagalog", "sequence_no": 55, "active": 1 },
             { "id": 156, "descriptor_id": 1, "value_code": "LAOTHER", "value_name": "Other", "sequence_no": 56, "active": 1 },

            // Alphabets
            { "id": 201, "descriptor_id": 2, "value_code": "AL001", "value_name": "Roman (Latin)", "sequence_no": 1, "active": 1 },
            { "id": 202, "descriptor_id": 2, "value_code": "AL002", "value_name": "Persian-Arabic", "sequence_no": 2, "active": 1 },
            { "id": 203, "descriptor_id": 2, "value_code": "AL003", "value_name": "Chinese", "sequence_no": 3, "active": 1 },
            { "id": 204, "descriptor_id": 2, "value_code": "AL004", "value_name": "Bengali", "sequence_no": 4, "active": 1 },
            { "id": 205, "descriptor_id": 2, "value_code": "AL005", "value_name": "Devangari", "sequence_no": 5, "active": 1 },
            { "id": 206, "descriptor_id": 2, "value_code": "AL006", "value_name": "Gujarati", "sequence_no": 6, "active": 1 },
            { "id": 207, "descriptor_id": 2, "value_code": "AL007", "value_name": "Punjabi", "sequence_no": 7, "active": 1 },
            { "id": 208, "descriptor_id": 2, "value_code": "AL008", "value_name": "Cyrillic", "sequence_no": 8, "active": 1 },
            { "id": 209, "descriptor_id": 2, "value_code": "AL009", "value_name": "Greek", "sequence_no": 9, "active": 1 },
            { "id": 210, "descriptor_id": 2, "value_code": "AL010", "value_name": "Hebrew", "sequence_no": 10, "active": 1 },
            { "id": 211, "descriptor_id": 2, "value_code": "AL011", "value_name": "Ge'ez", "sequence_no": 11, "active": 1 },
            { "id": 212, "descriptor_id": 2, "value_code": "AL012", "value_name": "Korean", "sequence_no": 12, "active": 1 },
            { "id": 213, "descriptor_id": 2, "value_code": "AL013", "value_name": "Japanese", "sequence_no": 13, "active": 1 },
            { "id": 214, "descriptor_id": 2, "value_code": "AL014", "value_name": "Tamil", "sequence_no": 14, "active": 1 },
            { "id": 215, "descriptor_id": 2, "value_code": "AL015", "value_name": "Thai", "sequence_no": 15, "active": 1 },
            { "id": 216, "descriptor_id": 2, "value_code": "AL016", "value_name": "Armenian", "sequence_no": 16, "active": 1 },
            { "id": 217, "descriptor_id": 2, "value_code": "ALOTHER", "value_name": "Other", "sequence_no": 17, "active": 1 },

            // Positions
            { "id": 300, "descriptor_id": 3, "value_code": "PO!!", "value_name": "-None Selected-", "sequence_no": 0, "active": 1 },
            { "id": 301, "descriptor_id": 3, "value_code": "PO001", "value_name": "Building: Outdoors", "sequence_no": 1, "active": 1 },
            { "id": 302, "descriptor_id": 3, "value_code": "PO002", "value_name": "Building: Indoors", "sequence_no": 2, "active": 1 },
            { "id": 303, "descriptor_id": 3, "value_code": "PO003", "value_name": "Market Stall", "sequence_no": 3, "active": 1 },
            { "id": 304, "descriptor_id": 3, "value_code": "PO004", "value_name": "Signpost/billboard/sandwichboard", "sequence_no": 4, "active": 1 },
            { "id": 305, "descriptor_id": 3, "value_code": "PO005", "value_name": "Mobile: external (car, taxi, bus etc)", "sequence_no": 5, "active": 1 },
            { "id": 306, "descriptor_id": 3, "value_code": "PO006", "value_name": "Mobile: internal (bus, tram etc)", "sequence_no": 6, "active": 1 },
            { "id": 307, "descriptor_id": 3, "value_code": "PO007", "value_name": "Letterbox", "sequence_no": 7, "active": 1 },
            { "id": 308, "descriptor_id": 3, "value_code": "PO008", "value_name": "Doorbell", "sequence_no": 8, "active": 1 },
            { "id": 309, "descriptor_id": 3, "value_code": "PO009", "value_name": "Other (lamppost, wall, bin etc)", "sequence_no": 9, "active": 1 },

            // Sign Types
            { "id": 400, "descriptor_id": 4, "value_code": "SI!!", "value_name": "-None Selected-", "sequence_no": 0, "active": 1 },
            { "id": 401, "descriptor_id": 4, "value_code": "SI001", "value_name": "Printed sign", "sequence_no": 1, "active": 1 },
            { "id": 402, "descriptor_id": 4, "value_code": "SI002", "value_name": "Printed poster", "sequence_no": 2, "active": 1 },
            { "id": 403, "descriptor_id": 4, "value_code": "SI003", "value_name": "Printed leaflet", "sequence_no": 3, "active": 1 },
            { "id": 404, "descriptor_id": 4, "value_code": "SI004", "value_name": "Printed label", "sequence_no": 4, "active": 1 },
            { "id": 405, "descriptor_id": 4, "value_code": "SI005", "value_name": "Handwritten note (unique)", "sequence_no": 5, "active": 1 },
            { "id": 406, "descriptor_id": 4, "value_code": "SI006", "value_name": "Handwritten leaflet/poster (photography)", "sequence_no": 6, "active": 1 },
            { "id": 407, "descriptor_id": 4, "value_code": "SI007", "value_name": "Electronic sign", "sequence_no": 7, "active": 1 },
            { "id": 408, "descriptor_id": 4, "value_code": "SI008", "value_name": "Graffiti", "sequence_no": 8, "active": 1 },

            // Outlets
            { "id": 500, "descriptor_id": 5, "value_code": "OU!!", "value_name": "-None Selected-", "sequence_no": 0, "active": 1 },
            { "id": 501, "descriptor_id": 5, "value_code": "OU001", "value_name": "Residential", "sequence_no": 1, "active": 1 },
            { "id": 502, "descriptor_id": 5, "value_code": "OU002", "value_name": "Restaurant", "sequence_no": 2, "active": 1 },
            { "id": 503, "descriptor_id": 5, "value_code": "OU003", "value_name": "Fast food/take away", "sequence_no": 3, "active": 1 },
            { "id": 504, "descriptor_id": 5, "value_code": "OU004", "value_name": "Grocery/corner shop", "sequence_no": 4, "active": 1 },
            { "id": 505, "descriptor_id": 5, "value_code": "OU005", "value_name": "Bakery/butcher", "sequence_no": 5, "active": 1 },
            { "id": 506, "descriptor_id": 5, "value_code": "OU006", "value_name": "General food/supermarket/minimarket ", "sequence_no": 6, "active": 1 },
            { "id": 507, "descriptor_id": 5, "value_code": "OU007", "value_name": "Cafe/pub/bar/shisha bar", "sequence_no": 7, "active": 1 },
            { "id": 508, "descriptor_id": 5, "value_code": "OU008", "value_name": "Community centre/cultural centre", "sequence_no": 8, "active": 1 },
            { "id": 509, "descriptor_id": 5, "value_code": "OU009", "value_name": "Religious institution", "sequence_no": 9, "active": 1 },
            { "id": 510, "descriptor_id": 5, "value_code": "OU010", "value_name": "Hairdresser/beauty salon/cosmetics", "sequence_no": 10, "active": 1 },
            { "id": 511, "descriptor_id": 5, "value_code": "OU011", "value_name": "Jewellery", "sequence_no": 11, "active": 1 },
            { "id": 512, "descriptor_id": 5, "value_code": "OU012", "value_name": "Souvenirs", "sequence_no": 12, "active": 1 },
            { "id": 513, "descriptor_id": 5, "value_code": "OU013", "value_name": "Tailor", "sequence_no": 13, "active": 1 },
            { "id": 514, "descriptor_id": 5, "value_code": "OU014", "value_name": "Clothing", "sequence_no": 14, "active": 1 },
            { "id": 515, "descriptor_id": 5, "value_code": "OU015", "value_name": "Stationary", "sequence_no": 15, "active": 1 },
            { "id": 516, "descriptor_id": 5, "value_code": "OU016", "value_name": "Laundrette", "sequence_no": 16, "active": 1 },
            { "id": 517, "descriptor_id": 5, "value_code": "OU017", "value_name": "Hardware/tools/gardening", "sequence_no": 17, "active": 1 },
            { "id": 518, "descriptor_id": 5, "value_code": "OU018", "value_name": "Bookshop", "sequence_no": 18, "active": 1 },
            { "id": 519, "descriptor_id": 5, "value_code": "OU019", "value_name": "PC/electronics/mobile phone shop", "sequence_no": 19, "active": 1 },
            { "id": 520, "descriptor_id": 5, "value_code": "OU020", "value_name": "Travel agent/cargo agent", "sequence_no": 20, "active": 1 },
            { "id": 521, "descriptor_id": 5, "value_code": "OU021", "value_name": "Money transfer agent", "sequence_no": 21, "active": 1 },
            { "id": 522, "descriptor_id": 5, "value_code": "OU022", "value_name": "Estate agent", "sequence_no": 22, "active": 1 },
            { "id": 523, "descriptor_id": 5, "value_code": "OU023", "value_name": "Pharmacy", "sequence_no": 23, "active": 1 },
            { "id": 524, "descriptor_id": 5, "value_code": "OU024", "value_name": "Lawyer/notary", "sequence_no": 24, "active": 1 },
            { "id": 525, "descriptor_id": 5, "value_code": "OU025", "value_name": "Accountant", "sequence_no": 25, "active": 1 },
            { "id": 526, "descriptor_id": 5, "value_code": "OU026", "value_name": "Doctor/dentist/clinic", "sequence_no": 26, "active": 1 },
            { "id": 527, "descriptor_id": 5, "value_code": "OU027", "value_name": "Car dealer/workshop/garage", "sequence_no": 27, "active": 1 },
            { "id": 528, "descriptor_id": 5, "value_code": "OU028", "value_name": "Petrol station", "sequence_no": 28, "active": 1 },
            { "id": 529, "descriptor_id": 5, "value_code": "OU029", "value_name": "Bank", "sequence_no": 29, "active": 1 },
            { "id": 530, "descriptor_id": 5, "value_code": "OU030", "value_name": "Post Office", "sequence_no": 30, "active": 1 },
            { "id": 531, "descriptor_id": 5, "value_code": "OU031", "value_name": "Government Office", "sequence_no": 31, "active": 1 },
            { "id": 532, "descriptor_id": 5, "value_code": "OU032", "value_name": "Company Office (insurance, etc)", "sequence_no": 32, "active": 1 },
            { "id": 533, "descriptor_id": 5, "value_code": "OU033", "value_name": "Street sign", "sequence_no": 33, "active": 1 },
            { "id": 534, "descriptor_id": 5, "value_code": "OU034", "value_name": "Traffic (road) sign/access instruction etc", "sequence_no": 34, "active": 1 },
            { "id": 535, "descriptor_id": 5, "value_code": "OU035", "value_name": "Public transport station (bus, train tram)", "sequence_no": 35, "active": 1 },
            { "id": 536, "descriptor_id": 5, "value_code": "OU036", "value_name": "Public transport vehicle (bus, train, tram)", "sequence_no": 36, "active": 1 },
            { "id": 537, "descriptor_id": 5, "value_code": "OU037", "value_name": "Airport/harbour", "sequence_no": 37, "active": 1 },
            { "id": 538, "descriptor_id": 5, "value_code": "OU038", "value_name": "Hospital", "sequence_no": 38, "active": 1 },
            { "id": 539, "descriptor_id": 5, "value_code": "OU039", "value_name": "School", "sequence_no": 39, "active": 1 },
            { "id": 540, "descriptor_id": 5, "value_code": "OU040", "value_name": "Preschool/nursery", "sequence_no": 40, "active": 1 },
            { "id": 541, "descriptor_id": 5, "value_code": "OU041", "value_name": "Museum", "sequence_no": 41, "active": 1 },
            { "id": 542, "descriptor_id": 5, "value_code": "OU042", "value_name": "Library", "sequence_no": 42, "active": 1 },
            { "id": 543, "descriptor_id": 5, "value_code": "OU043", "value_name": "Sports/leisure centre", "sequence_no": 43, "active": 1 },
            { "id": 544, "descriptor_id": 5, "value_code": "OU044", "value_name": "University", "sequence_no": 44, "active": 1 },
            { "id": 545, "descriptor_id": 5, "value_code": "OU045", "value_name": "ATM", "sequence_no": 45, "active": 1 },
            { "id": 546, "descriptor_id": 5, "value_code": "OU046", "value_name": "Parking meter", "sequence_no": 46, "active": 1 },
            { "id": 547, "descriptor_id": 5, "value_code": "OU047", "value_name": "Billboard", "sequence_no": 47, "active": 1 },
             { "id": 548, "descriptor_id": 5, "value_code": "OUOTHER", "value_name": "Other", "sequence_no": 48, "active": 1 },

            // Designs
            
            { "id": 601, "descriptor_id": 6, "value_code": "DE001", "value_name": "Icon/logo", "sequence_no": 1, "active": 1 },
            { "id": 602, "descriptor_id": 6, "value_code": "DE002", "value_name": "Product image/Service image", "sequence_no": 2, "active": 1 },
            { "id": 603, "descriptor_id": 6, "value_code": "DE003", "value_name": "Accompanying image", "sequence_no": 3, "active": 1 },

            // Content
            
            { "id": 701, "descriptor_id": 7, "value_code": "CO001", "value_name": "Outlet name/institution name/personal name", "sequence_no": 1, "active": 1 },
            { "id": 702, "descriptor_id": 7, "value_code": "CO002", "value_name": "Location name/street name", "sequence_no": 2, "active": 1 },
            { "id": 703, "descriptor_id": 7, "value_code": "CO003", "value_name": "Outlet information (opening times etc)", "sequence_no": 3, "active": 1 },
            { "id": 704, "descriptor_id": 7, "value_code": "CO004", "value_name": "Product brand name", "sequence_no": 4, "active": 1 },
            { "id": 705, "descriptor_id": 7, "value_code": "CO005", "value_name": "Service brand name", "sequence_no": 5, "active": 1 },
            { "id": 706, "descriptor_id": 7, "value_code": "CO006", "value_name": "Product information", "sequence_no": 6, "active": 1 },
            { "id": 707, "descriptor_id": 7, "value_code": "CO007", "value_name": "Service information", "sequence_no": 7, "active": 1 },
            { "id": 708, "descriptor_id": 7, "value_code": "CO008", "value_name": "Safety/health instruction", "sequence_no": 8, "active": 1 },
            { "id": 709, "descriptor_id": 7, "value_code": "CO009", "value_name": "Event information", "sequence_no": 9, "active": 1 },
            { "id": 710, "descriptor_id": 7, "value_code": "CO010", "value_name": "Religious message/political message", "sequence_no": 10, "active": 1 },
            { "id": 711, "descriptor_id": 7, "value_code": "CO011", "value_name": "Personal message", "sequence_no": 11, "active": 1 },
            { "id": 712, "descriptor_id": 7, "value_code": "CO012", "value_name": "Other non-commercial information", "sequence_no": 12, "active": 1 },

            // Sectors
            { "id": 800, "descriptor_id": 8, "value_code": "SE!!", "value_name": "-None Selected-", "sequence_no": 0, "active": 1 },
            { "id": 801, "descriptor_id": 8, "value_code": "SE001", "value_name": "Public", "sequence_no": 1, "active": 1 },
            { "id": 802, "descriptor_id": 8, "value_code": "SE002", "value_name": "Private", "sequence_no": 2, "active": 1 },
            { "id": 803, "descriptor_id": 8, "value_code": "SE003", "value_name": "Voluntary", "sequence_no": 3, "active": 1 },

            // Audiences
            { "id": 900, "descriptor_id": 9, "value_code": "AU!!", "value_name": "-None Selected-", "sequence_no": 0, "active": 1 },
            { "id": 901, "descriptor_id": 9, "value_code": "AU001", "value_name": "Exclusive", "sequence_no": 1, "active": 1 },
            { "id": 902, "descriptor_id": 9, "value_code": "AU002", "value_name": "Inclusive", "sequence_no": 2, "active": 1 },

            // Purposes
            
            { "id": 1001, "descriptor_id": 10, "value_code": "PU001", "value_name": "Communicative", "sequence_no": 1, "active": 1 },
            { "id": 1002, "descriptor_id": 10, "value_code": "PU002", "value_name": "Emblematic", "sequence_no": 2, "active": 1 },

            // Functions
            
            { "id": 1101, "descriptor_id": 11, "value_code": "FU001", "value_name": "Landmark", "sequence_no": 1, "active": 1 },
            { "id": 1102, "descriptor_id": 11, "value_code": "FU002", "value_name": "Recruitment/information", "sequence_no": 2, "active": 1 },
            { "id": 1103, "descriptor_id": 11, "value_code": "FU003", "value_name": "Public statement", "sequence_no": 3, "active": 1 },
            { "id": 1104, "descriptor_id": 11, "value_code": "FU004", "value_name": "Muted", "sequence_no": 4, "active": 1 },

            // Arrangements
            
            { "id": 1201, "descriptor_id": 12, "value_code": "AR001", "value_name": "Duplicating", "sequence_no": 1, "active": 1 },
            { "id": 1202, "descriptor_id": 12, "value_code": "AR002", "value_name": "Fragmentary", "sequence_no": 2, "active": 1 },
            { "id": 1203, "descriptor_id": 12, "value_code": "AR003", "value_name": "Overlapping", "sequence_no": 3, "active": 1 },
            { "id": 1204, "descriptor_id": 12, "value_code": "AR004", "value_name": "Complementary", "sequence_no": 4, "active": 1 },

            //  One language dominant?
            { "id": 1301, "descriptor_id": 13, "value_code": "ON001", "value_name": "", "sequence_no": 1, "active": 1 },


            // Dominance
            
            { "id": 1401, "descriptor_id": 14, "value_code": "DO001", "value_name": "Position", "sequence_no": 1, "active": 1 },
            { "id": 1402, "descriptor_id": 14, "value_code": "DO002", "value_name": "Font size", "sequence_no": 2, "active": 1 },
            { "id": 1403, "descriptor_id": 14, "value_code": "DO003", "value_name": "Colour", "sequence_no": 3, "active": 1 },
            { "id": 1404, "descriptor_id": 14, "value_code": "DO004", "value_name": "Other", "sequence_no": 5, "active": 1 },
            { "id": 1405, "descriptor_id": 14, "value_code": "DO005", "value_name": "Quantity", "sequence_no": 4, "active": 1 },

            //  Number of alphabets
            { "id": 1501, "descriptor_id": 15, "value_code": "NA001", "value_name": "", "sequence_no": 1, "active": 1 },

            //  Number of languages
            { "id": 1601, "descriptor_id": 16, "value_code": "NL001", "value_name": "", "sequence_no": 1, "active": 1 }
        ]

        
    }

    

    


    // createTables()
    // Create the tables in the database.
    this.createTables = function () {
        var def1 = new $.Deferred();
        var self = this;
        this.linguaDB.transaction(function (tx) {

            if ($("#rebuild").val() === "yes") {
                tx.executeSql('DROP TABLE IF EXISTS descriptors');
                tx.executeSql('DROP TABLE IF EXISTS descriptorvalues');
                //tx.executeSql('DROP TABLE IF EXISTS config');
                tx.executeSql('DROP TABLE IF EXISTS photorecords');
                tx.executeSql('DROP TABLE IF EXISTS photodescriptors');
                tx.executeSql('DROP TABLE IF EXISTS phototranslations');
            }

            // create the tables
            tx.executeSql('CREATE TABLE IF NOT EXISTS descriptors (id integer primary key, code text, descriptor_name text, other_flag integer, multi_select integer, if_yes_display integer)');
            tx.executeSql('CREATE TABLE IF NOT EXISTS descriptorvalues (id integer primary key, descriptor_id integer, value_code text, value_name text, sequence_no integer, active integer)');
            tx.executeSql('CREATE TABLE IF NOT EXISTS photorecords (id integer primary key, unique_id text, title text, latitude real, longitude real, date_created text, image_data text, notes text)');
            tx.executeSql('CREATE TABLE IF NOT EXISTS photodescriptors (id integer primary key, photo_record_id integer, descriptor_code text, descriptor_value_id integer, other_text text, link_key text )');
            tx.executeSql('CREATE TABLE IF NOT EXISTS phototranslations (id integer primary key, photo_record_id integer, translation text, link_key text )');
            tx.executeSql('CREATE TABLE IF NOT EXISTS config (key text, value text)');

            // populate the data

            // descriptors
            for (var i = 0; i < self.descriptorData.descriptors.length; i++) {
                tx.executeSql('UPDATE descriptors set id = ?, descriptor_name = ?, other_flag = ?, multi_select = ?, if_yes_display = ? WHERE code = ? ',
                    [self.descriptorData.descriptors[i].id, self.descriptorData.descriptors[i].description, self.descriptorData.descriptors[i].other, self.descriptorData.descriptors[i].multi,
                    self.descriptorData.descriptors[i].if_yes_display, self.descriptorData.descriptors[i].code]);

                tx.executeSql('INSERT INTO descriptors (id, code, descriptor_name, other_flag, multi_select, if_yes_display) SELECT ?, ?, ?,?, ?, ? WHERE NOT EXISTS(SELECT changes() AS change FROM descriptors WHERE change <> 0);',
                    [self.descriptorData.descriptors[i].id, self.descriptorData.descriptors[i].code, self.descriptorData.descriptors[i].description, self.descriptorData.descriptors[i].other,
                        self.descriptorData.descriptors[i].multi, self.descriptorData.descriptors[i].if_yes_display ]);
            }

            // descriptorvalues
            for (var i = 0; i < self.descriptorData.descriptor_values.length; i++) {
                tx.executeSql('UPDATE descriptorvalues set id = ?, descriptor_id = ?, value_name = ?, sequence_no = ?, active = ? WHERE value_code = ? ',
                    [self.descriptorData.descriptor_values[i].id, self.descriptorData.descriptor_values[i].descriptor_id, self.descriptorData.descriptor_values[i].value_name,
                        self.descriptorData.descriptor_values[i].sequence_no, self.descriptorData.descriptor_values[i].active, self.descriptorData.descriptor_values[i].value_code]);

                tx.executeSql('INSERT INTO descriptorvalues (id, descriptor_id, value_code, value_name, sequence_no, active) SELECT ?, ?, ?,?, ?, ? WHERE NOT EXISTS(SELECT changes() AS change FROM descriptorvalues WHERE change <> 0);',
                    [self.descriptorData.descriptor_values[i].id, self.descriptorData.descriptor_values[i].descriptor_id, self.descriptorData.descriptor_values[i].value_code,
                        self.descriptorData.descriptor_values[i].value_name, self.descriptorData.descriptor_values[i].sequence_no, self.descriptorData.descriptor_values[i].active]);
            }


            // update the version number of the app's database
            var promise = self.configDB.updateValue("version", self.appVersion);

            promise.fail(function () {
                self.phoneGapUtils.showAlert("error creating database");
                def1.reject();
            });

            promise.done(function () {
                def1.resolve();
            });
            

        }, function (e) {
            // Error in creating tables.
            self.phoneGapUtils.showAlert("error creating database");
            def1.reject();
        });

        return def1.promise();
    }



    // populate()
    // Check to see if the database exists and if not create it.
    this.populate = function () {
        var def2 = new $.Deferred();
        var self = this;

        var promise = this.configDB.checkVersionCorrect(this.appVersion);

        promise.fail(function () {
            // version incorrect so create tables then resolve populate() deferral
            self.createTables().then(function () {
                def2.resolve();
            });
        });

        promise.done(function () {
            // version correct so just resolve deferral
            def2.resolve();
            
        });

        return def2.promise();
    }

    }