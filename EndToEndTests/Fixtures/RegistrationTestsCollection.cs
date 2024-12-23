using EndToEndTests.Fixtures;
using Xunit;

namespace EndToEndTests.Collections;

// Define a test collection
[CollectionDefinition("Registration Tests Collection")]
public class RegistrationTestsCollection : ICollectionFixture<RegistrationTestsCollectionFixture>
{
    // No additional implementation needed
}