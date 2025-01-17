namespace Z80Sharp.Tests
{
    static class Constants
    {
        internal static string[] UndocumentedInstructions = 
        {
            "ed63", "ed6b", "ed70", "ed71", // MISC table

            "cb30", "cb31", "cb32", "cb33", "cb34", "cb35", "cb36", "cb37", // BIT table

            "dd80", "dd81", "dd82", "dd83", "dd84", "dd85", "dd87", // INDEX X table
            "dd88", "dd89", "dd8a", "dd8b", "dd8c", "dd8d", "dd8f",
            "dd90", "dd91", "dd92", "dd93", "dd94", "dd95", "dd97",
            "dd98", "dd99", "dd9a", "dd9b", "dd9c", "dd9d", "dd9f",
            "dda0", "dda1", "dda2", "dda3", "dda4", "dda5", "dda7",
            "dda8", "dda9", "ddaa", "ddab", "ddac", "ddad", "ddaf",
            "ddb0", "ddb1", "ddb2", "ddb3", "ddb4", "ddb5", "ddb7",
            "ddb8", "ddb9", "ddba", "ddbb", "ddbc", "ddbd", "ddbf",
            "dd06", "dd0e", "dd16", "dd1e", "dd26", "dd2e", "dd3e",
            "dd40", "dd41", "dd42", "dd43", "dd44", "dd45", "dd47",

            "fd80", "fd81", "fd82", "fd83", "fd84", "fd85", "fd87", // INDEX Y table
            "fd88", "fd89", "fd8a", "fd8b", "fd8c", "fd8d", "fd8f",
            "fd90", "fd91", "fd92", "fd93", "fd94", "fd95", "fd97",
            "fd98", "fd99", "fd9a", "fd9b", "fd9c", "fd9d", "fd9f",
            "fda0", "fda1", "fda2", "fda3", "fda4", "fda5", "fda7",
            "fda8", "fda9", "fdaa", "fdab", "fdac", "fdad", "fdaf",
            "fdb0", "fdb1", "fdb2", "fdb3", "fdb4", "fdb5", "fdb7",
            "fdb8", "fdb9", "fdba", "fdbb", "fdbc", "fdbd", "fdbf",
            "fd06", "fd0e", "fd16", "fd1e", "fd26", "fd2e", "fd3e",
            "fd40", "fd41", "fd42", "fd43", "fd44", "fd45", "fd47",

            "ddcb0", "ddcb1", "ddcb2", "ddcb3", "ddcb4", "ddcb5", "ddcb7",          // INDEX X BIT table
            "ddcb10", "ddcb11", "ddcb12", "ddcb13", "ddcb14", "ddcb15", "ddcb17",
            "ddcb8", "ddcb9", "ddcba", "ddcbb", "ddcbc", "ddcbd", "ddcbf",
            "ddcb18", "ddcb19", "ddcb1a", "ddcb1b", "ddcb1c", "ddcb1d", "ddcb1f",
            "ddcb20", "ddcb21", "ddcb22", "ddcb23", "ddcb24", "ddcb25", "ddcb27",
            "ddcb30", "ddcb31", "ddcb32", "ddcb33", "ddcb34", "ddcb35", "ddcb37",
            "ddcb28", "ddcb29", "ddcb2a", "ddcb2b", "ddcb2c", "ddcb2d", "ddcb2f",
            "ddcb38", "ddcb39", "ddcb3a", "ddcb3b", "ddcb3c", "ddcb3d", "ddcb3f",
            "ddcb40", "ddcb41", "ddcb42", "ddcb43", "ddcb44", "ddcb45", "ddcb47",
            "ddcb48", "ddcb49", "ddcb4a", "ddcb4b", "ddcb4c", "ddcb4d", "ddcb4f",
            "ddcb50", "ddcb51", "ddcb52", "ddcb53", "ddcb54", "ddcb55", "ddcb57",
            "ddcb58", "ddcb59", "ddcb5a", "ddcb5b", "ddcb5c", "ddcb5d", "ddcb5f",
            "ddcb60", "ddcb61", "ddcb62", "ddcb63", "ddcb64", "ddcb65", "ddcb67",
            "ddcb68", "ddcb69", "ddcb6a", "ddcb6b", "ddcb6c", "ddcb6d", "ddcb6f",
            "ddcb70", "ddcb71", "ddcb72", "ddcb73", "ddcb74", "ddcb75", "ddcb77",
            "ddcb78", "ddcb79", "ddcb7a", "ddcb7b", "ddcb7c", "ddcb7d", "ddcb7f",
            "ddcb80", "ddcb81", "ddcb82", "ddcb83", "ddcb84", "ddcb85", "ddcb87",
            "ddcb88", "ddcb89", "ddcb8a", "ddcb8b", "ddcb8c", "ddcb8d", "ddcb8f",
            "ddcb90", "ddcb91", "ddcb92", "ddcb93", "ddcb94", "ddcb95", "ddcb97",
            "ddcb98", "ddcb99", "ddcb9a", "ddcb9b", "ddcb9c", "ddcb9d", "ddcb9f",
            "ddcba0", "ddcba1", "ddcba2", "ddcba3", "ddcba4", "ddcba5", "ddcba7",
            "ddcba8", "ddcba9", "ddcbaa", "ddcbab", "ddcbac", "ddcbad", "ddcbaf",
            "ddcbb0", "ddcbb1", "ddcbb2", "ddcbb3", "ddcbb4", "ddcbb5", "ddcbb7",
            "ddcbb8", "ddcbb9", "ddcbba", "ddcbbb", "ddcbbc", "ddcbbd", "ddcbbf",
            "ddcbc0", "ddcbc1", "ddcbc2", "ddcbc3", "ddcbc4", "ddcbc5", "ddcbc7",
            "ddcbc8", "ddcbc9", "ddcbca", "ddcbcb", "ddcbcc", "ddcbcd", "ddcbcf",
            "ddcbd0", "ddcbd1", "ddcbd2", "ddcbd3", "ddcbd4", "ddcbd5", "ddcbd7",
            "ddcbd8", "ddcbd9", "ddcbda", "ddcbdb", "ddcbdc", "ddcbdd", "ddcbdf",
            "ddcbe0", "ddcbe1", "ddcbe2", "ddcbe3", "ddcbe4", "ddcbe5", "ddcbe7",
            "ddcbe8", "ddcbe9", "ddcbea", "ddcbeb", "ddcbec", "ddcbed", "ddcbef",
            "ddcbf0", "ddcbf1", "ddcbf2", "ddcbf3", "ddcbf4", "ddcbf5", "ddcbf7",
            "ddcbf8", "ddcbf9", "ddcbfa", "ddcbfb", "ddcbfc", "ddcbfd", "ddcbff",

            "fdcb0", "fdcb1", "fdcb2", "fdcb3", "fdcb4", "fdcb5", "fdcb7",          // INDEX X BIT table
            "fdcb10", "fdcb11", "fdcb12", "fdcb13", "fdcb14", "fdcb15", "fdcb17",
            "fdcb8", "fdcb9", "fdcba", "fdcbb", "fdcbc", "fdcbd", "fdcbf",
            "fdcb18", "fdcb19", "fdcb1a", "fdcb1b", "fdcb1c", "fdcb1d", "fdcb1f",
            "fdcb20", "fdcb21", "fdcb22", "fdcb23", "fdcb24", "fdcb25", "fdcb27",
            "fdcb30", "fdcb31", "fdcb32", "fdcb33", "fdcb34", "fdcb35", "fdcb37",
            "fdcb28", "fdcb29", "fdcb2a", "fdcb2b", "fdcb2c", "fdcb2d", "fdcb2f",
            "fdcb38", "fdcb39", "fdcb3a", "fdcb3b", "fdcb3c", "fdcb3d", "fdcb3f",
            "fdcb40", "fdcb41", "fdcb42", "fdcb43", "fdcb44", "fdcb45", "fdcb47",
            "fdcb48", "fdcb49", "fdcb4a", "fdcb4b", "fdcb4c", "fdcb4d", "fdcb4f",
            "fdcb50", "fdcb51", "fdcb52", "fdcb53", "fdcb54", "fdcb55", "fdcb57",
            "fdcb58", "fdcb59", "fdcb5a", "fdcb5b", "fdcb5c", "fdcb5d", "fdcb5f",
            "fdcb60", "fdcb61", "fdcb62", "fdcb63", "fdcb64", "fdcb65", "fdcb67",
            "fdcb68", "fdcb69", "fdcb6a", "fdcb6b", "fdcb6c", "fdcb6d", "fdcb6f",
            "fdcb70", "fdcb71", "fdcb72", "fdcb73", "fdcb74", "fdcb75", "fdcb77",
            "fdcb78", "fdcb79", "fdcb7a", "fdcb7b", "fdcb7c", "fdcb7d", "fdcb7f",
            "fdcb80", "fdcb81", "fdcb82", "fdcb83", "fdcb84", "fdcb85", "fdcb87",
            "fdcb88", "fdcb89", "fdcb8a", "fdcb8b", "fdcb8c", "fdcb8d", "fdcb8f",
            "fdcb90", "fdcb91", "fdcb92", "fdcb93", "fdcb94", "fdcb95", "fdcb97",
            "fdcb98", "fdcb99", "fdcb9a", "fdcb9b", "fdcb9c", "fdcb9d", "fdcb9f",
            "fdcba0", "fdcba1", "fdcba2", "fdcba3", "fdcba4", "fdcba5", "fdcba7",
            "fdcba8", "fdcba9", "fdcbaa", "fdcbab", "fdcbac", "fdcbad", "fdcbaf",
            "fdcbb0", "fdcbb1", "fdcbb2", "fdcbb3", "fdcbb4", "fdcbb5", "fdcbb7",
            "fdcbb8", "fdcbb9", "fdcbba", "fdcbbb", "fdcbbc", "fdcbbd", "fdcbbf",
            "fdcbc0", "fdcbc1", "fdcbc2", "fdcbc3", "fdcbc4", "fdcbc5", "fdcbc7",
            "fdcbc8", "fdcbc9", "fdcbca", "fdcbcb", "fdcbcc", "fdcbcd", "fdcbcf",
            "fdcbd0", "fdcbd1", "fdcbd2", "fdcbd3", "fdcbd4", "fdcbd5", "fdcbd7",
            "fdcbd8", "fdcbd9", "fdcbda", "fdcbdb", "fdcbdc", "fdcbfd", "fdcbdf",
            "fdcbe0", "fdcbe1", "fdcbe2", "fdcbe3", "fdcbe4", "fdcbe5", "fdcbe7",
            "fdcbe8", "fdcbe9", "fdcbea", "fdcbeb", "fdcbec", "fdcbed", "fdcbef",
            "fdcbf0", "fdcbf1", "fdcbf2", "fdcbf3", "fdcbf4", "fdcbf5", "fdcbf7",
            "fdcbf8", "fdcbf9", "fdcbfa", "fdcbfb", "fdcbfc", "fdcbfd", "fdcbff",
        };
    }
}
