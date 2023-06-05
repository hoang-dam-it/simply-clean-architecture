using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace Architecture.Tests
{
    public class ArchitectureTests
    {
        private const string DomainNamespace = "Shop.Domain";
        private const string ApplicationNamespace = "Shop.Application";
        private const string InfrastructureNamespace = "Shop.Infrastructure";
        private const string PersistenceNamespace = "Shop.Persistence";
        private const string WebApiNamespace = "Shop.Api";

        [Fact]
        public void Domain_Should_All_Have_Sealed_Class()
        {
            // Arrange
            var domainAssembly = GetAssemblyByProjectName(DomainNamespace);


            // Act
            var testResult = Types
                .InAssembly(domainAssembly)
                .That()
                .AreClasses()
                .Should()
                .BeSealed()
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Domain_Should_Not_HaveDependencyOnOtherProjects()
        {
            // Arrange
            var assembly = GetAssemblyByProjectName(DomainNamespace);

            var otherProjects = new[]
            {
                ApplicationNamespace,
                InfrastructureNamespace,
            };

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(otherProjects)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Application_Should_Not_HaveDependencyOnOtherProjects()
        {
            // Arrange
            var assembly = GetAssemblyByProjectName(ApplicationNamespace);

            var otherProjects = new[]
            {
                InfrastructureNamespace,
            };

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(otherProjects)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Persistence_Should_Not_HaveDependencyOnOtherProjects()
        {
            // Arrange
            var assembly = GetAssemblyByProjectName(PersistenceNamespace);

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .That()
                .HaveNameEndingWith("Repository")
                .Should()
                .HaveDependencyOn(DomainNamespace)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Handler_Should_HaveDependencyOnDomain()
        {
            // Arrange
            var assembly = GetAssemblyByProjectName(ApplicationNamespace);

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .That()
                .HaveNameEndingWith("Handler")
                .Should()
                .HaveDependencyOn(DomainNamespace)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Controller_Should_HaveDependencyOnMediaR()
        {
            // Arrange
            var assembly = GetAssemblyByProjectName(WebApiNamespace);

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .That()
                .HaveNameEndingWith("Controller")
                .Should()
                .HaveDependencyOn("MediatR")
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        private Assembly GetAssemblyByProjectName(string projectName)
        {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            return
                loadedAssemblies
                    .FirstOrDefault(assembly =>
                        !assembly.IsDynamic &&
                        !string.IsNullOrEmpty(assembly.Location) &&
                        assembly.GetName().Name == projectName) ??
                throw new FileNotFoundException($"No assembly could be found with {projectName} project name.");
        }
    }
}
