using System;
using System.Linq;

namespace Quantify.Estimates.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for byte arrays to compare SQL Server rowversion values.
    /// </summary>
    public static class ByteArrayRowVersionExtensions
    {
        /// <summary>
        /// Compares the current byte array (representing a rowversion) with another rowversion byte array.
        /// A later version will have a higher numerical value.
        /// </summary>
        /// <param name="currentVersion">The current rowversion byte array (this instance).</param>
        /// <param name="otherVersion">The other rowversion byte array to compare against.</param>
        /// <returns>
        ///   -1 if currentVersion is earlier than otherVersion.
        ///    0 if currentVersion is the same as otherVersion.
        ///    1 if currentVersion is later than otherVersion.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown if either byte array is not 8 bytes long.</exception>
        public static int CompareRowVersion(this byte[] currentVersion, byte[] otherVersion)
        {
            if (currentVersion == null || currentVersion.Length != 8)
            {
                throw new ArgumentException("The currentVersion byte array must be 8 bytes long.", nameof(currentVersion));
            }
            if (otherVersion == null || otherVersion.Length != 8)
            {
                throw new ArgumentException("The otherVersion byte array must be 8 bytes long.", nameof(otherVersion));
            }

            // Create copies to avoid modifying the original arrays if Array.Reverse is used.
            byte[] v1Copy = (byte[])currentVersion.Clone();
            byte[] v2Copy = (byte[])otherVersion.Clone();

            // Reverse bytes for consistent ulong conversion on little-endian systems
            // to correctly interpret the SQL Server rowversion's numerical value.
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(v1Copy);
                Array.Reverse(v2Copy);
            }

            ulong ulongVersion1 = BitConverter.ToUInt64(v1Copy, 0);
            ulong ulongVersion2 = BitConverter.ToUInt64(v2Copy, 0);

            return ulongVersion1.CompareTo(ulongVersion2);
        }

        /// <summary>
        /// Checks if the current rowversion byte array is later than another rowversion.
        /// </summary>
        /// <param name="currentVersion">The current rowversion byte array (this instance).</param>
        /// <param name="otherVersion">The other rowversion byte array to compare against.</param>
        /// <returns>True if currentVersion is later, false otherwise.</returns>
        public static bool IsRowVersionLaterThan(this byte[] currentVersion, byte[] otherVersion)
        {
            return currentVersion.CompareRowVersion(otherVersion) > 0;
        }

        /// <summary>
        /// Checks if the current rowversion byte array is earlier than another rowversion.
        /// </summary>
        /// <param name="currentVersion">The current rowversion byte array (this instance).</param>
        /// <param name="otherVersion">The other rowversion byte array to compare against.</param>
        /// <returns>True if currentVersion is earlier, false otherwise.</returns>
        public static bool IsRowVersionEarlierThan(this byte[] currentVersion, byte[] otherVersion)
        {
            return currentVersion.CompareRowVersion(otherVersion) < 0;
        }

        /// <summary>
        /// Checks if the current rowversion byte array is the same as another rowversion.
        /// </summary>
        /// <param name="currentVersion">The current rowversion byte array (this instance).</param>
        /// <param name="otherVersion">The other rowversion byte array to compare against.</param>
        /// <returns>True if currentVersion is the same, false otherwise.</returns>
        public static bool IsRowVersionSameAs(this byte[] currentVersion, byte[] otherVersion)
        {
            return currentVersion.CompareRowVersion(otherVersion) == 0;
        }
    }
}
