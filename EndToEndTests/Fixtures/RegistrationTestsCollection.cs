using EndToEndTests.Fixtures;
using Xunit;

namespace EndToEndTests.Collections;

[CollectionDefinition("Registration Tests Collection")]
public class RegistrationTestsCollection : ICollectionFixture<RegistrationTestsCollectionFixture>
{
    // No additional implementation needed as of now
}