// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;
using System.Linq;

namespace System.IO.Tests
{
    public class Directory_GetFileSystemEntries_str_str : Directory_GetFileSystemEntries_str
    {
        #region Utilities

        public override string[] GetEntries(string dirName)
        {
            return Directory.GetFileSystemEntries(dirName, "*");
        }

        public virtual string[] GetEntries(string dirName, string searchPattern)
        {
            return Directory.GetFileSystemEntries(dirName, searchPattern);
        }

        #endregion

        #region UniversalTests

        [Fact]
        public void SearchPatternNull()
        {
            Assert.Throws<ArgumentNullException>(() => GetEntries(TestDirectory, null));
        }

        [Fact]
        public void SearchPatternEmpty()
        {
            // To avoid OS differences we have decided not to throw an argument exception when empty
            // string passed. But we should return 0 items.
            Assert.Empty(GetEntries(TestDirectory, string.Empty));
        }

        [Fact]
        public void SearchPatternValid()
        {
            Assert.Empty(GetEntries(TestDirectory, "a..b abc..d")); //Should not throw
        }

        [Fact]
        public void SearchPatternDotIsStar()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            testDir.CreateSubdirectory("TestDir1");
            testDir.CreateSubdirectory("TestDir2");
            using (File.Create(Path.Combine(testDir.FullName, "TestFile1")))
            using (File.Create(Path.Combine(testDir.FullName, "TestFile2")))
            {
                string[] strArr = GetEntries(testDir.FullName, ".");
                if (TestFiles)
                {
                    Assert.Contains(Path.Combine(testDir.FullName, "TestFile1"), strArr);
                    Assert.Contains(Path.Combine(testDir.FullName, "TestFile2"), strArr);
                }
                if (TestDirectories)
                {
                    Assert.Contains(Path.Combine(testDir.FullName, "TestDir1"), strArr);
                    Assert.Contains(Path.Combine(testDir.FullName, "TestDir2"), strArr);
                }
            }
        }

        [Fact]
        public void SearchPatternWithTrailingStar()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            testDir.CreateSubdirectory("TestDir1");
            testDir.CreateSubdirectory("TestDir2");
            testDir.CreateSubdirectory("TestDir3");
            using (File.Create(Path.Combine(testDir.FullName, "TestFile1")))
            using (File.Create(Path.Combine(testDir.FullName, "TestFile2")))
            using (File.Create(Path.Combine(testDir.FullName, "Test1File2")))
            using (File.Create(Path.Combine(testDir.FullName, "Test1Dir2")))
            {
                string[] strArr = GetEntries(testDir.FullName, "Test1*");
                if (TestFiles)
                { 
                    Assert.Contains(Path.Combine(testDir.FullName, "Test1File2"), strArr);
                    Assert.Contains(Path.Combine(testDir.FullName, "Test1Dir2"), strArr);
                }

                strArr = GetEntries(testDir.FullName, "*");
                if (TestFiles)
                {
                    Assert.Contains(Path.Combine(testDir.FullName, "TestFile1"), strArr);
                    Assert.Contains(Path.Combine(testDir.FullName, "TestFile2"), strArr);
                    Assert.Contains(Path.Combine(testDir.FullName, "Test1File2"), strArr);
                    Assert.Contains(Path.Combine(testDir.FullName, "Test1Dir2"), strArr);
                }
                if (TestDirectories)
                {
                    Assert.Contains(Path.Combine(testDir.FullName, "TestDir1"), strArr);
                    Assert.Contains(Path.Combine(testDir.FullName, "TestDir2"), strArr);
                    Assert.Contains(Path.Combine(testDir.FullName, "TestDir3"), strArr);
                }
            }
        }

        [Fact]
        public void SearchPatternWithLeadingStar()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            testDir.CreateSubdirectory("TestDir1");
            testDir.CreateSubdirectory("TestDir2");
            testDir.CreateSubdirectory("TestDir3");
            using (File.Create(Path.Combine(testDir.FullName, "TestFile1")))
            using (File.Create(Path.Combine(testDir.FullName, "TestFile2")))
            using (File.Create(Path.Combine(testDir.FullName, "Test1File2")))
            using (File.Create(Path.Combine(testDir.FullName, "Test1Dir2")))
            {
                string[] strArr = GetEntries(testDir.FullName, "*2");
                if (TestFiles)
                {
                    Assert.Contains(Path.Combine(testDir.FullName, "Test1Dir2"), strArr);
                    Assert.Contains(Path.Combine(testDir.FullName, "Test1File2"), strArr);
                    Assert.Contains(Path.Combine(testDir.FullName, "TestFile2"), strArr);
                }
                if (TestDirectories)
                {
                    Assert.Contains(Path.Combine(testDir.FullName, "TestDir2"), strArr);
                }

                strArr = GetEntries(testDir.FullName, "*Dir*");
                if (TestFiles)
                {
                    Assert.Contains(Path.Combine(testDir.FullName, "Test1Dir2"), strArr);
                }
                if (TestDirectories)
                {
                    Assert.Contains(Path.Combine(testDir.FullName, "TestDir1"), strArr);
                    Assert.Contains(Path.Combine(testDir.FullName, "TestDir2"), strArr);
                    Assert.Contains(Path.Combine(testDir.FullName, "TestDir3"), strArr);
                }
            }
        }

        [Fact]
        public void SearchPatternByExtension()
        {
            if (TestFiles)
            {
                DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
                using (File.Create(Path.Combine(testDir.FullName, "TestFile1.txt")))
                using (File.Create(Path.Combine(testDir.FullName, "TestFile2.xxt")))
                using (File.Create(Path.Combine(testDir.FullName, "Test1File2.txt")))
                using (File.Create(Path.Combine(testDir.FullName, "Test1Dir2.txx")))
                {
                    string[] strArr = GetEntries(testDir.FullName, "*.txt");
                    Assert.Equal(2, strArr.Length);
                    Assert.Contains(Path.Combine(testDir.FullName, "TestFile1.txt"), strArr);
                    Assert.Contains(Path.Combine(testDir.FullName, "Test1File2.txt"), strArr);
                }
            }
        }

        [Fact]
        public void SearchPatternExactMatch()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            Directory.CreateDirectory(Path.Combine(testDir.FullName, "AAA"));
            Directory.CreateDirectory(Path.Combine(testDir.FullName, "AAAB"));
            Directory.CreateDirectory(Path.Combine(testDir.FullName, "CAAA"));
            using (File.Create(Path.Combine(testDir.FullName, "AAABB")))
            using (File.Create(Path.Combine(testDir.FullName, "AAABBC")))
            using (File.Create(Path.Combine(testDir.FullName, "CAAABB")))
            {
                if (TestFiles)
                {
                    string[] results = GetEntries(testDir.FullName, "AAABB");
                    Assert.Equal(1, results.Length);
                    Assert.Contains(Path.Combine(testDir.FullName, "AAABB"), results);
                }
                if (TestDirectories)
                {
                    string[] results = GetEntries(testDir.FullName, "AAA");
                    Assert.Equal(1, results.Length);
                    Assert.Contains(Path.Combine(testDir.FullName, "AAA"), results);
                }
            }
        }

        [Fact]
        public void SearchPatternIgnoreSubDirectories()
        {
            //Shouldn't get files on full path by default
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            Directory.CreateDirectory(Path.Combine(testDir.FullName, GetTestFileName()));
            using (File.Create(Path.Combine(testDir.FullName, GetTestFileName())))
            using (File.Create(Path.Combine(TestDirectory, GetTestFileName())))
            {
                string[] results = GetEntries(TestDirectory, Path.Combine(testDir.Name, "*"));
                if (TestDirectories && TestFiles)
                    Assert.Equal(2, results.Length);
                else
                    Assert.Equal(1, results.Length);
            }
        }

        #endregion

        #region PlatformSpecific

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]  // Long path segment in search pattern throws PathTooLongException
        public void WindowsSearchPatternLongSegment()
        {
            // Create a path segment longer than the normal max of 255
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            string longName = new string('k', 257);

            Assert.Throws<PathTooLongException>(() => GetEntries(testDir.FullName, longName));
        }

        [ConditionalFact(nameof(AreAllLongPathsAvailable))]
        [ActiveIssue(20117, TargetFrameworkMonikers.Uap)]
        public void SearchPatternLongPath()
        {
            // Create a destination path longer than the traditional Windows limit of 256 characters
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());

            string longName = new string('k', 254);
            string longFullname = Path.Combine(testDir.FullName, longName);

            if (TestFiles)
            {
                using (File.Create(longFullname)) { }
            }
            else
            {
                Directory.CreateDirectory(longFullname);
            }

            string[] results = GetEntries(testDir.FullName, longName);
            Assert.Contains(longFullname, results);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]  // Search pattern with double dots throws ArgumentException
        public void WindowsSearchPatternWithDoubleDots()
        {
            Assert.Throws<ArgumentException>(() => GetEntries(TestDirectory, Path.Combine("..ab ab.. .. abc..d", "abc..")));
            Assert.Throws<ArgumentException>(() => GetEntries(TestDirectory, ".."));
            Assert.Throws<ArgumentException>(() => GetEntries(TestDirectory, @".." + Path.DirectorySeparatorChar));
        }

        private static char[] OldWildcards = new char[] { '*', '?' };
        private static char[] NewWildcards = new char[] { '<', '>', '\"' };

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]  // Windows-invalid search patterns throw
        public void WindowsSearchPatternInvalid()
        {
            Assert.Throws<ArgumentException>(() => GetEntries(TestDirectory, "\0"));
            Assert.Throws<ArgumentException>(() => GetEntries(TestDirectory, "|"));

            Assert.All(Path.GetInvalidFileNameChars().Except(OldWildcards).Except(NewWildcards), invalidChar =>
            {
                switch (invalidChar)
                {
                    case '\\':
                    case '/':
                        Assert.Throws<DirectoryNotFoundException>(() => GetEntries(Directory.GetCurrentDirectory(), string.Format("te{0}st", invalidChar.ToString())));
                        break;
                    //We don't throw in V1 too
                    case ':':
                        //History:
                        // 1) we assumed that this will work in all non-9x machine
                        // 2) Then only in XP
                        // 3) NTFS?
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                            FileSystemDebugInfo.IsCurrentDriveNTFS()) // testing NTFS
                        {
                            Assert.Throws<IOException>(() => GetEntries(Directory.GetCurrentDirectory(), string.Format("te{0}st", invalidChar.ToString())));
                        }
                        else
                        {
                            GetEntries(Directory.GetCurrentDirectory(), string.Format("te{0}st", invalidChar.ToString()));
                        }
                        break;
                    default:
                        Assert.Throws<ArgumentException>(() => GetEntries(Directory.GetCurrentDirectory(), string.Format("te{0}st", invalidChar.ToString())));
                        break;
                }
            });
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]  // Windows-invalid search patterns throw
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "In netcoreapp we made three new characters be treated as valid wildcards instead of invalid characters. NetFX still treats them as InvalidChars.")]
        public void WindowsSearchPatternInvalid_Wildcards_netcoreapp()
        {
            Assert.All(OldWildcards, invalidChar =>
            {
                GetEntries(Directory.GetCurrentDirectory(), string.Format("te{0}st", invalidChar.ToString()));
            });
            Assert.All(NewWildcards, invalidChar =>
            {
                GetEntries(Directory.GetCurrentDirectory(), string.Format("te{0}st", invalidChar.ToString()));
            });
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]  // Windows-invalid search patterns throw
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "In netcoreapp we made three new characters be treated as valid wildcards instead of invalid characters. NetFX still treats them as InvalidChars.")]
        public void WindowsSearchPatternInvalid_Wildcards_netfx()
        {
            Assert.All(OldWildcards, invalidChar =>
            {
                GetEntries(Directory.GetCurrentDirectory(), string.Format("te{0}st", invalidChar.ToString()));
            });
            Assert.All(NewWildcards, invalidChar =>
            {
                Assert.Throws<ArgumentException>(() => GetEntries(Directory.GetCurrentDirectory(), string.Format("te{0}st", invalidChar.ToString())));
            });
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Unix-invalid sarch patterns throw ArgumentException
        public void UnixSearchPatternInvalid()
        {
            Assert.Throws<ArgumentException>(() => GetEntries(TestDirectory, "\0"));
            Assert.Throws<ArgumentException>(() => GetEntries(TestDirectory, string.Format("te{0}st", "\0".ToString())));
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]  // ? in search pattern returns results
        public virtual void WindowsSearchPatternQuestionMarks()
        {
            string testDir1Str = GetTestFileName();
            DirectoryInfo testDir = new DirectoryInfo(TestDirectory);
            DirectoryInfo testDir1 = testDir.CreateSubdirectory(testDir1Str);

            using (File.Create(Path.Combine(TestDirectory, testDir1Str, GetTestFileName())))
            using (File.Create(Path.Combine(TestDirectory, GetTestFileName())))
            {
                string[] results = GetEntries(TestDirectory, string.Format("{0}.???", new string('?', GetTestFileName().Length)));
                if (TestFiles && TestDirectories)
                    Assert.Equal(2, results.Length);
                else
                    Assert.Equal(1, results.Length);
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]  // Whitespace in search pattern returns nothing
        public void WindowsSearchPatternWhitespace()
        {
            Assert.Empty(GetEntries(TestDirectory, "           "));
            Assert.Empty(GetEntries(TestDirectory, "\n"));
            Assert.Empty(GetEntries(TestDirectory, " "));
            Assert.Empty(GetEntries(TestDirectory, "\t"));
        }

        [Fact]
        [PlatformSpecific(CaseSensitivePlatforms)]
        public void SearchPatternCaseSensitive()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            string testBase = GetTestFileName();
            testDir.CreateSubdirectory(testBase + "aBBb");
            testDir.CreateSubdirectory(testBase + "aBBB");

            File.Create(Path.Combine(testDir.FullName, testBase + "AAAA")).Dispose();
            File.Create(Path.Combine(testDir.FullName, testBase + "aAAa")).Dispose();
            
            if (TestDirectories)
            {
                Assert.Equal(2, GetEntries(testDir.FullName, "*BB*").Length);
            }
            if (TestFiles)
            {
                Assert.Equal(2, GetEntries(testDir.FullName, "*AA*").Length);
            }
        }

        [Fact]
        [PlatformSpecific(CaseInsensitivePlatforms)]
        public void SearchPatternCaseInsensitive()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            string testBase = GetTestFileName();
            testDir.CreateSubdirectory(testBase + "yZZz");
            testDir.CreateSubdirectory(testBase + "yZZZ");

            File.Create(Path.Combine(testDir.FullName, testBase + "YYYY")).Dispose();
            File.Create(Path.Combine(testDir.FullName, testBase + "yYYy")).Dispose();

            if (TestDirectories)
            {
                Assert.Equal(1, GetEntries(testDir.FullName, "*ZZ*").Length);
            }
            if (TestFiles)
            {
                Assert.Equal(1, GetEntries(testDir.FullName, "*YY*").Length);
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Unix-valid chars in file search patterns
        public void UnixSearchPatternFileValidChar()
        {
            if (TestFiles)
            {
                DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
                foreach (string valid in WindowsInvalidUnixValid)
                    File.Create(Path.Combine(testDir.FullName, valid)).Dispose();

                foreach (string valid in WindowsInvalidUnixValid)
                    Assert.Contains(Path.Combine(testDir.FullName, valid), GetEntries(testDir.FullName, valid));
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Unix-valid chars in directory search patterns
        public void UnixSearchPatternDirectoryValidChar()
        {
            if (TestDirectories)
            {
                DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
                foreach (string valid in WindowsInvalidUnixValid)
                    testDir.CreateSubdirectory(valid);

                foreach (string valid in WindowsInvalidUnixValid)
                    Assert.Contains(Path.Combine(testDir.FullName, valid), GetEntries(testDir.FullName, valid));
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Search pattern with DoubleDots on Unix
        public void UnixSearchPatternWithDoubleDots()
        {
            // search pattern is valid but directory doesn't exist
            Assert.Throws<DirectoryNotFoundException>(() => GetEntries(TestDirectory, Path.Combine("..ab ab.. .. abc..d", "abc..")));

            // invalid search pattern trying to go up a directory with ..
            Assert.Throws<ArgumentException>(() => GetEntries(TestDirectory, ".."));
            Assert.Throws<ArgumentException>(() => GetEntries(TestDirectory, @".." + Path.DirectorySeparatorChar));
            Assert.Throws<ArgumentException>(() => GetEntries(TestDirectory, Path.Combine("..ab ab.. .. abc..d", "abc", "..")));
            Assert.Throws<ArgumentException>(() => GetEntries(TestDirectory, Path.Combine("..ab ab.. .. abc..d", "..", "abc")));
            Assert.Throws<ArgumentException>(() => GetEntries(TestDirectory, Path.Combine("..", "..ab ab.. .. abc..d", "abc")));
            Assert.Throws<ArgumentException>(() => GetEntries(TestDirectory, Path.Combine("..", "..ab ab.. .. abc..d", "abc") + Path.DirectorySeparatorChar));
        }

        #endregion
    }
}
