using System;
using StartItUp.Profile;
using StartItUp.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Test
{
    [TestClass]
    public class StartupManagerTest
    {
        //[TestMethod]
        //public void T1()
        //{
        //    Profile[] stubItems = new Profile[]
        //    {
        //        new Profile()
        //        {
        //            IsEnabled = true,
        //            Extension = new Extension(@"A\A"),
        //            Profile = new Profile()
        //            {
        //                Name = "A",
        //                Description = "A"
        //            }
        //        },

        //        new Profile()
        //        {
        //            IsEnabled = true,
        //            Extension = new Extension(@"B\B"),
        //            Profile = new Profile()
        //            {
        //                Name = "B",
        //                Description = "B"
        //            }
        //        },

        //        new Profile()
        //        {
        //            IsEnabled = true,
        //            Extension = new Extension(@"C\C"),
        //            Profile = new Profile()
        //            {
        //                Name = "C",
        //                Description = "C"
        //            }
        //        }
        //    };

        //    ProfileManager startupManager = new ProfileManager();

        //    foreach(var startupItem in stubItems)
        //    {
        //        startupManager.Items.Add(startupItem);
        //    }

        //    startupManager.Save();

        //    startupManager.Items.Clear();

        //    startupManager.Load();
        //    Assert.AreEqual(startupManager.Items.Count, stubItems.Length);

        //    for(int i = 0; i < startupManager.Items.Count; i++)
        //    {
        //        var item = startupManager.Items[i];
        //        var stubItem = stubItems[i];

        //        Assert.AreEqual(item.IsEnabled, stubItem.IsEnabled);
        //        Assert.AreEqual(item.Extension.Dir, stubItem.Extension.Dir);
        //        Assert.AreEqual(item.Profile.Name, stubItem.Profile.Name);
        //        Assert.AreEqual(item.Profile.Description, stubItem.Profile.Description);
        //    }

        //    Profile firstStubItem = stubItems.First();
        //    firstStubItem.Profile.Name = @"P@P\P@P";

        //    var firstItem = startupManager.Items.First();
        //    firstItem.Profile.Name = firstStubItem.Profile.Name;

        //    startupManager.Save();
        //    startupManager.Load();

        //    Assert.AreEqual(startupManager.Items.Count, stubItems.Length);

        //    Assert.AreEqual(firstItem.IsEnabled, firstStubItem.IsEnabled);
        //    Assert.AreEqual(firstItem.Extension.Dir, firstStubItem.Extension.Dir);
        //    Assert.AreEqual(firstItem.Profile.Name, firstStubItem.Profile.Name);
        //    Assert.AreEqual(firstItem.Profile.Description, firstStubItem.Profile.Description);
        //}
    }
}
