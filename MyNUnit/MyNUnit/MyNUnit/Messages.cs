namespace MyNUnit
{
    /// <summary>
    /// Messages that user of MyNUnit can receive
    /// </summary>
    public static class Messages
    {
        public const string TestMustBePublic = "Test method must be public";
        public const string TestMustBeInstance = "Test method must be instance";
        public const string TestMustBeVoid = "Test method must have void return type";
        public const string TestMustHaveNoParameters = "Test must have no parameters";
        public const string Empty = "";
        public const string FixtureMethodMustBePublicOrInternal = "Fixture methods, maked with Before and After attributes, must be public or internal";
        public const string FixtureMethodMustBeInstance = "Fixture methods, maked with Before and After attributes, must be instance, not static";
        public const string FixtureMethodMustBeVoid = "Fixture methods, maked with Before and After attributes, must have void return type";
        public const string FixtureMethodMustHaveNoParameters = "Fixture methods, maked with Before and After attributes, must have no parameters";
        public const string StaticFixtureMethodMustBePublicOrInternal = "Static fixture method must be public or internal";
        public const string StaticFixtureMethodMustBeStatic = "Static fixture method must be static, not instance";
        public const string StaticFixtureMethodMustBeVoid = "Static fixture method must have void return type";
        public const string StaticFixtureMethodMustHaveNoParameters = "Static fixture method must have no parameters";
    }
}
