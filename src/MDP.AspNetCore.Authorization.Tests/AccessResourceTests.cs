using System;
using System.Collections.Generic;
using Xunit;

namespace MDP.AspNetCore.Authorization.Tests
{
    public class AccessResourceTests
    {
        // Methods
        [Theory]
        [InlineData("https://localhost/Home/Index", "https://localhost/Home/Index", true)]
        [InlineData("https://localhost/home/index", "https://localhost/Home/Index", true)]
        [InlineData("https://localhost/Home/Index", "https://localhost/Home/Other", false)]
        [InlineData("https://localhost/Home/Index", "https://otherhost/Home/Index", false)]
        [InlineData("https://localhost/Home/Index", "http://localhost/Home/Index", false)]
        [InlineData("https://localhost/Home/Index", "https://localhost/Home", false)]
        [InlineData("https://localhost/Home", "https://localhost/Home/Index", false)]
        public void HasAccess_String(string resourceUri, string accessUri, bool expected)
        {
            // Arrange
            var roleAssignment = new RoleAssignment("Admin");
            var accessPermission = new AccessPermission("Admin", new List<string>(), accessUri);
            var accessResource = new AccessResource(resourceUri);

            // Act
            var actual = accessResource.HasAccess(roleAssignment, accessPermission);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("https://localhost/Home/Index", "https://localhost/Home/*", true)]
        [InlineData("https://localhost/Home/Index/123", "https://localhost/Home/*", true)]
        [InlineData("https://localhost/Home", "https://localhost/Home/*", false)]
        [InlineData("https://localhost/User/Index", "https://localhost/Home/*", false)]
        [InlineData("https://localhost/Home/Index/123", "https://localhost/Home/*/123", true)]
        [InlineData("https://localhost/Home/Index/456", "https://localhost/Home/*/123", false)]
        public void HasAccess_Wildcard(string resourceUri, string accessUri, bool expected)
        {
            // Arrange
            var roleAssignment = new RoleAssignment("Admin");
            var accessPermission = new AccessPermission("Admin", new List<string>(), accessUri);
            var accessResource = new AccessResource(resourceUri);

            // Act
            var actual = accessResource.HasAccess(roleAssignment, accessPermission);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("123456", "https://localhost/123456/Home/Index", "https://localhost/[TenantId]/Home/*", true)]
        [InlineData("123456", "https://localhost/654321/Home/Index", "https://localhost/[TenantId]/Home/*", false)]
        [InlineData("123456", "https://localhost/123456/User/Index", "https://localhost/[TenantId]/Home/*", false)]
        [InlineData("123456", "https://localhost/123456/Home/Index", "https://localhost/[OtherId]/Home/*", false)]
        [InlineData("123456", "https://localhost/123456/Home/Index", "https://localhost/[]/Home/*", false)]
        public void HasAccess_Scope(string tenantId, string resourceUri, string accessUri, bool expected)
        {
            // Arrange
            var roleAssignment = new RoleAssignment("Admin", new List<string>() { tenantId });
            var accessPermission = new AccessPermission("Admin", new List<string>() { "TenantId" }, accessUri);
            var accessResource = new AccessResource(resourceUri);

            // Act
            var actual = accessResource.HasAccess(roleAssignment, accessPermission);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("123456", "https://localhost/123456/Home/Index", "https://localhost/[TenantId=123456]/Home/*", true)]
        [InlineData("654321", "https://localhost/654321/Home/Index", "https://localhost/[TenantId=123456]/Home/*", false)]
        [InlineData("123456", "https://localhost/654321/Home/Index", "https://localhost/[TenantId=123456]/Home/*", false)]
        [InlineData("654321", "https://localhost/123456/Home/Index", "https://localhost/[TenantId=123456]/Home/*", false)]
        [InlineData("123456", "https://localhost/123456/User/Index", "https://localhost/[TenantId=123456]/Home/*", false)]
        [InlineData("123456", "https://localhost/123456/Home/Index", "https://localhost/[TenantId=]/Home/*", false)]
        [InlineData("123456", "https://localhost/123456/Home/Index", "https://localhost/[=123456]/Home/*", false)]
        public void HasAccess_ScopeValue(string tenantId, string resourceUri, string accessUri, bool expected)
        {
            // Arrange
            var roleAssignment = new RoleAssignment("Admin", new List<string>() { tenantId });
            var accessPermission = new AccessPermission("Admin", new List<string>() { "TenantId" }, accessUri);
            var accessResource = new AccessResource(resourceUri);

            // Act
            var actual = accessResource.HasAccess(roleAssignment, accessPermission);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HasAccess_RoleIdMismatch_Throws()
        {
            // Arrange
            var roleAssignment = new RoleAssignment("User");
            var accessPermission = new AccessPermission("Admin", new List<string>(), "https://localhost/Home/*");
            var accessResource = new AccessResource("https://localhost/Home/Index");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => accessResource.HasAccess(roleAssignment, accessPermission));
        }

        [Fact]
        public void HasAccess_RoleScopesCountMismatch_Throws()
        {
            // Arrange
            var roleAssignment = new RoleAssignment("Admin");
            var accessPermission = new AccessPermission("Admin", new List<string>() { "TenantId" }, "https://localhost/[TenantId]/Home/*");
            var accessResource = new AccessResource("https://localhost/123456/Home/Index");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => accessResource.HasAccess(roleAssignment, accessPermission));
        }
    }
}
