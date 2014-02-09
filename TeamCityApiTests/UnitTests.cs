using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TeamCity;

namespace TeamCityApiTests
{
    [TestFixture]
    public class UnitTests
    {
        private TeamCityApi api;

        [TestFixtureSetUp]
        public void Setup()
        {
            api = new TeamCityApi("http://localhost:88", "admin", "test");
        }


        [Test]
        public void CanAuthenticate()
        {
            bool success = api.Authenticate();
            Assert.That(api.Authenticate(), "can't authenticate");
        }

        [Test]
        public void ListsAtLeastOneProject()
        {
            var projects = api.GetProjects();
            Assert.That(projects.Count > 0);
        }
        
        [Test]
        public void CanGetProjectNames()
        {
            var projects = api.GetProjects();
            var projectNames = projects.Select(x => x.Name).ToList();

            Assert.That(projects.All(x=> x.Name!=null));
        }

        [Test]
        public void CanGetBuildsForProjects()
        {
            var projects = api.GetProjects();

            var testProject = projects.First();
            var builds = testProject.GetBuilds();


            Assert.That(builds != null && builds.Any());
        }

        [Test]
        public void NoBuildTypeShowsUpMoreThanOnceForRecentBuildsFromProjects()
        {
            var projects = api.GetProjects();

            var testProject = projects.First();
            var builds = testProject.GetMostRecentBuilds();

            HashSet<Build> buildHash = new HashSet<Build>();

            Assert.That(builds.All(b=> buildHash.Add(b)));
        }


        [Test]
        public void CanGetBuildConfigurationsForProject()
        {
            var projects = api.GetProjects();

            foreach(var project in projects)
            {
                var buildTypes = project.GetBuildTypes();
                if (buildTypes.Any())
                {
                    var builds = buildTypes.First().GetBuilds();

                    builds.ForEach(b => Assert.That(b.Name != null));
                }
            }
        }

        [Test]
        public void CanGetBuildDetailsFromBuild()
        {
            var projects = api.GetProjects();
            foreach (var project in projects)
            {
                var builds = project.GetBuilds();
                if (builds.Any())
                {
                    var buildDetails = builds.First().GetBuildDetails();

                    Assert.That(buildDetails != null && buildDetails.Name != null && buildDetails.ProjectName != null && buildDetails.StatusText != null);
                    break;
                }
            }
        }


    }
}
