// Original from https://github.com/ThomasMiz/RectpackSharp

using System;
using System.Collections.Generic;
using Murder.Core.Geometry;

namespace Murder.Editor.Data.Graphics
{

    /// <summary>
    /// Specifies hints that help optimize the rectangle packing algorithm. 
    /// </summary>
    [Flags]
    public enum PackingHints
    {
        /// <summary>Tells the rectangle packer to try inserting the rectangles ordered by area.</summary>
        TryByArea = 1,

        /// <summary>Tells the rectangle packer to try inserting the rectangles ordered by perimeter.</summary>
        TryByPerimeter = 2,

        /// <summary>Tells the rectangle packer to try inserting the rectangles ordered by bigger side.</summary>
        TryByBiggerSide = 4,

        /// <summary>Tells the rectangle packer to try inserting the rectangles ordered by width.</summary>
        TryByWidth = 8,

        /// <summary>Tells the rectangle packer to try inserting the rectangles ordered by height.</summary>
        TryByHeight = 16,

        /// <summary>Tells the rectangle packer to try inserting the rectangles ordered by a pathological multiplier.</summary>
        TryByPathologicalMultiplier = 32,

        /// <summary>Specifies to try all the possible hints, as to find the best packing configuration.</summary>
        FindBest = TryByArea | TryByPerimeter | TryByBiggerSide | TryByWidth | TryByHeight | TryByPathologicalMultiplier,

        /// <summary>Specifies hints to optimize for rectangles who have one side much bigger than the other.</summary>
        UnusualSizes = TryByPerimeter | TryByBiggerSide | TryByPathologicalMultiplier,

        /// <summary>Specifies hints to optimize for rectangles whose sides are relatively similar.</summary>
        MostlySquared = TryByArea | TryByBiggerSide | TryByWidth | TryByHeight,
    }

    
    /// <summary>
    /// Provides internal values and functions used by the rectangle packing algorithm.
    /// </summary>
    internal static class PackingHintExtensions
    {
        /// <summary>
        /// Represents a method for calculating a sort key from a <see cref="PackingRectangle"/>.
        /// </summary>
        /// <param name="rectangle">The <see cref="PackingRectangle"/> whose sort key to calculate.</param>
        /// <returns>The value that should be assigned to <see cref="PackingRectangle.SortKey"/>.</returns>
        private delegate uint GetSortKeyDelegate(in PackingRectangle rectangle);

        /// <summary>The maximum amount of hints that can be specified by a <see cref="PackingHint"/>.</summary>
        internal const int MaxHintCount = 6;

        public static uint GetArea(in PackingRectangle rectangle) => rectangle.Area;
        public static uint GetPerimeter(in PackingRectangle rectangle) => rectangle.Perimeter;
        public static uint GetBiggerSide(in PackingRectangle rectangle) => rectangle.BiggerSide;
        public static uint GetWidth(in PackingRectangle rectangle) => rectangle.Width;
        public static uint GetHeight(in PackingRectangle rectangle) => rectangle.Height;
        public static uint GetPathologicalMultiplier(in PackingRectangle rectangle) => rectangle.PathologicalMultiplier;

        /// <summary>
        /// Separates a <see cref="PackingHint"/> into the multiple options it contains,
        /// saving each of those separately onto a <see cref="Span{T}"/>.
        /// </summary>
        /// <param name="packingHint">The <see cref="PackingHint"/> to separate.</param>
        /// <param name="span">The span in which to write the resulting hints. This span's excess will be sliced.</param>
        public static void GetFlagsFrom(PackingHints packingHint, ref Span<PackingHints> span)
        {
            int index = 0;
            if (packingHint.HasFlag(PackingHints.TryByArea))
                span[index++] = PackingHints.TryByArea;
            if (packingHint.HasFlag(PackingHints.TryByPerimeter))
                span[index++] = PackingHints.TryByPerimeter;
            if (packingHint.HasFlag(PackingHints.TryByBiggerSide))
                span[index++] = PackingHints.TryByBiggerSide;
            if (packingHint.HasFlag(PackingHints.TryByWidth))
                span[index++] = PackingHints.TryByWidth;
            if (packingHint.HasFlag(PackingHints.TryByHeight))
                span[index++] = PackingHints.TryByHeight;
            if (packingHint.HasFlag(PackingHints.TryByPathologicalMultiplier))
                span[index++] = PackingHints.TryByPathologicalMultiplier;
            span = span.Slice(0, index);
        }

        /// <summary>
        /// Sorts the given <see cref="PackingRectangle"/> array using the specified <see cref="PackingHint"/>.
        /// </summary>
        /// <param name="rectangles">The rectangles to sort.</param>
        /// <param name="packingHint">The hint to sort by. Must be a single bit value.</param>
        /// <remarks>
        /// The <see cref="PackingRectangle.SortKey"/> values will be modified.
        /// </remarks>
        public static void SortByPackingHint(Span<PackingRectangle> rectangles, PackingHints packingHint)
        {
            // We first get the appropiate delegate for getting a rectangle's sort key.
            GetSortKeyDelegate getKeyDelegate;
            switch (packingHint)
            {
                case PackingHints.TryByArea:
                    getKeyDelegate = GetArea;
                    break;
                case PackingHints.TryByPerimeter:
                    getKeyDelegate = GetPerimeter;
                    break;
                case PackingHints.TryByBiggerSide:
                    getKeyDelegate = GetBiggerSide;
                    break;
                case PackingHints.TryByWidth:
                    getKeyDelegate = GetWidth;
                    break;
                case PackingHints.TryByHeight:
                    getKeyDelegate = GetHeight;
                    break;
                case PackingHints.TryByPathologicalMultiplier:
                    getKeyDelegate = GetPathologicalMultiplier;
                    break;
                default:
                    throw new ArgumentException(nameof(packingHint));
            };

            // We use the getKeyDelegate to set the sort keys for all the rectangles.
            for (int i = 0; i < rectangles.Length; i++)
                rectangles[i].SortKey = getKeyDelegate(rectangles[i]);

            // We sort the array, using the default rectangle comparison (which compares sort keys).
            rectangles.Sort();
        }
    }

    
    public class PackingException : Exception
    {
        public PackingException() : base() { }

        public PackingException(string message) : base(message) { }

        public PackingException(string message,  Exception innerException) : base(message, innerException) { }
    }
    
    
    /// <summary>
    /// A rectangle that can be used for a rectangle packing operation.
    /// </summary>
    public struct PackingRectangle : IEquatable<PackingRectangle>, IComparable<PackingRectangle>
    {
        /// <summary>
        /// A value that can be used to identify this <see cref="PackingRectangle"/>. This value is
        /// never touched by the rectangle packing algorithm.
        /// </summary>
        public int Id;

        /// <summary>A value used internally by the packing algorithm for sorting rectangles.</summary>
        public uint SortKey;

        /// <summary>The X coordinate of the left edge of this <see cref="PackingRectangle"/>.</summary>
        public uint X;

        /// <summary>The Y coordinate of the top edge of this <see cref="PackingRectangle"/>.</summary>
        public uint Y;

        /// <summary>The width of this <see cref="PackingRectangle"/>.</summary>
        public uint Width;

        /// <summary>The height of this <see cref="PackingRectangle"/>.</summary>
        public uint Height;

        /// <summary>
        /// Gets or sets the X coordinate of the right edge of this <see cref="PackingRectangle"/>.
        /// </summary>
        /// <remarks>Setting this will only modify the <see cref="Width"/>.</remarks>
        public uint Right
        {
            get => X + Width;
            set => Width = value - X;
        }

        /// <summary>
        /// Gets or sets the Y coordinate of the bottom edge of this <see cref="PackingRectangle"/>.
        /// </summary>
        /// <remarks>Setting this will only modify the <see cref="Height"/>.</remarks>
        public uint Bottom
        {
            get => Y + Height;
            set => Height = value - Y;
        }

        /// <summary>Calculates this <see cref="PackingRectangle"/>'s area.</summary>
        public uint Area => Width * Height;

        /// <summary>Calculates this <see cref="PackingRectangle"/>'s perimeter.</summary>
        public uint Perimeter => Width + Width + Height + Height;

        /// <summary>Gets this <see cref="PackingRectangle"/>'s bigger side.</summary>
        public uint BiggerSide => Math.Max(Width, Height);

        /// <summary>Calculates this <see cref="PackingRectangle"/>'s pathological multiplier.</summary>
        /// <remarks>This is calculated as: <code>max(width, height) / min(width, height) * width * height</code></remarks>
        public uint PathologicalMultiplier => (Width > Height ? (Width / Height) : (Height / Width)) * Width * Height;

        /// <summary>
        /// Creates a <see cref="PackingRectangle"/> with the specified values.
        /// </summary>
        public PackingRectangle(uint x, uint y, uint width, uint height, int id = 0)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Id = id;
            SortKey = 0;
        }

        /// <summary>
        /// Creates a <see cref="PackingRectangle"/> from a <see cref="Rectangle"/>.
        /// </summary>
        public PackingRectangle(Rectangle rectangle, int id = 0)
            : this((uint)rectangle.X, (uint)rectangle.Y, (uint)rectangle.Width, (uint)rectangle.Height, id)
        {

        }

        public static implicit operator Rectangle(PackingRectangle rectangle)
            => new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);

        public static implicit operator PackingRectangle(Rectangle rectangle)
            => new PackingRectangle((uint)rectangle.X, (uint)rectangle.Y, (uint)rectangle.Width, (uint)rectangle.Height);

        public static bool operator ==(PackingRectangle left, PackingRectangle right) => left.Equals(right);
        public static bool operator !=(PackingRectangle left, PackingRectangle right) => !left.Equals(right);

        /// <summary>
        /// Returns whether the given <see cref="PackingRectangle"/> is contained
        /// entirely within this <see cref="PackingRectangle"/>.
        /// </summary>
        public bool Contains(in PackingRectangle other)
        {
            return X <= other.X && Y <= other.Y && Right >= other.Right && Bottom >= other.Bottom;
        }

        /// <summary>
        /// Returns whether the given <see cref="PackingRectangle"/> intersects with
        /// this <see cref="PackingRectangle"/>.
        /// </summary>
        public bool Intersects(in PackingRectangle other)
        {
            return other.X < X + Width && X < (other.X + other.Width)
                && other.Y < Y + Height && Y < other.Y + other.Height;
        }

        /// <summary>
        /// Calculates the intersection of this <see cref="PackingRectangle"/> with another.
        /// </summary>
        public PackingRectangle Intersection(in PackingRectangle other)
        {
            uint x1 = Math.Max(X, other.X);
            uint x2 = Math.Min(Right, other.Right);
            uint y1 = Math.Max(Y, other.Y);
            uint y2 = Math.Min(Bottom, other.Bottom);

            if (x2 >= x1 && y2 >= y1)
                return new PackingRectangle(x1, y1, x2 - x1, y2 - y1);
            return default;
        }

        public override string ToString()
        {
            return string.Concat("{ X=", X.ToString(), ", Y=", Y.ToString(), ", Width=", Width.ToString() + ", Height=", Height.ToString(), ", Id=", Id.ToString(), " }");
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Width, Height, Id);
        }

        public bool Equals(PackingRectangle other)
        {
            return X == other.X && Y == other.Y && Width == other.Width
                && Height == other.Height && Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            if (obj is PackingRectangle viewport)
            {
                return Equals(viewport);
            }

            return false;
        }

        /// <summary>
        /// Compares this <see cref="SortKey"/> with another <see cref="PackingRectangle"/>'s.
        /// </summary>
        public int CompareTo(PackingRectangle other)
        {
            return -SortKey.CompareTo(other.SortKey);
        }
    }
    
    
    /// <summary>
    /// A static class providing functionality for packing rectangles into a bin as small as possible.
    /// </summary>
    public static class RectanglePacker
    {
        /// <summary>A weak reference to the last list used, so it can be reused in subsequent packs.</summary>
        private static WeakReference<List<PackingRectangle>?>? _oldListReference = null;
        private static readonly object _oldListReferenceLock = new();

        /// <summary>
        /// Finds a way to pack all the given rectangles into a single bin. Performance can be traded for
        /// space efficiency by using the optional parameters.
        /// </summary>
        /// <param name="rectangles">The rectangles to pack. The result is saved onto this array.</param>
        /// <param name="bounds">The bounds of the resulting bin. This will always be at X=Y=0.</param>
        /// <param name="packingHint">Specifies hints for optimizing performance.</param>
        /// <param name="acceptableDensity">Searching stops once a bin is found with this density (usedArea/boundsArea) or better.</param>
        /// <param name="stepSize">The amount by which to increment/decrement size when trying to pack another bin.</param>
        /// <param name="maxBoundsWidth">The maximum allowed width for the resulting bin, or null for no limit.</param>
        /// <param name="maxBoundsHeight">The maximum allowed height for the resulting bin, or null for no limit.</param>
        /// <remarks>
        /// The <see cref="PackingRectangle.Id"/> values are never touched. Use this to identify your rectangles.
        /// </remarks>
        public static void Pack(Span<PackingRectangle> rectangles, out PackingRectangle bounds,
            PackingHints packingHint = PackingHints.FindBest, double acceptableDensity = 1, uint stepSize = 1,
            uint? maxBoundsWidth = null, uint? maxBoundsHeight = null)
        {
            if (rectangles == null)
                throw new ArgumentNullException(nameof(rectangles));

            if (stepSize == 0)
                throw new ArgumentOutOfRangeException(nameof(stepSize), stepSize, nameof(stepSize) + " must be greater than 0.");

            if (double.IsNaN(acceptableDensity) || double.IsInfinity(acceptableDensity))
                throw new ArgumentException("Must be a real number", nameof(acceptableDensity));

            if (maxBoundsWidth != null && maxBoundsWidth.Value == 0)
                throw new ArgumentOutOfRangeException(nameof(maxBoundsWidth), maxBoundsWidth, nameof(maxBoundsWidth) + " must be greater than 0.");

            if (maxBoundsHeight != null && maxBoundsHeight.Value == 0)
                throw new ArgumentOutOfRangeException(nameof(maxBoundsHeight), maxBoundsHeight, nameof(maxBoundsHeight) + " must be greater than 0.");

            bounds = default;
            if (rectangles.Length == 0)
                return;

            // We separate the value in packingHint into the different options it specifies.
            Span<PackingHints> hints = stackalloc PackingHints[PackingHintExtensions.MaxHintCount];
            PackingHintExtensions.GetFlagsFrom(packingHint, ref hints);

            if (hints.Length == 0)
                throw new ArgumentException("No valid packing hints specified.", nameof(packingHint));

            // We'll try uint.MaxValue as initial bin size. The packing algoritm already tries to
            // use as little space as possible, so this will be QUICKLY cut down closer to the
            // final bin size.
            uint binWidth = maxBoundsWidth.GetValueOrDefault(uint.MaxValue);
            uint binHeight = maxBoundsHeight.GetValueOrDefault(uint.MaxValue);

            // We turn the acceptableDensity parameter into an acceptableArea value, so we can
            // compare the area directly rather than having to calculate the density each time.
            uint rectanglesAreaSum = CalculateTotalArea(rectangles);
            double acceptableBoundsAreaTmp = Math.Ceiling(rectanglesAreaSum / acceptableDensity);
            uint acceptableBoundsArea = (acceptableBoundsAreaTmp <= 0) ? rectanglesAreaSum :
                double.IsPositiveInfinity(acceptableBoundsAreaTmp) ? uint.MaxValue :
                (uint)acceptableBoundsAreaTmp;

            // We get a list that will be used (and reused) by the packing algorithm.
            List<PackingRectangle> emptySpaces = GetList(rectangles.Length * 2);

            // We'll store the area of the best solution so far here.
            uint currentBestArea = uint.MaxValue;
            bool hasSolution = false;

            // In one array we'll store the current best solution, and we'll also need two temporary arrays.
            Span<PackingRectangle> currentBest = rectangles;
            Span<PackingRectangle> tmpBest = new PackingRectangle[rectangles.Length];
            Span<PackingRectangle> tmpArray = new PackingRectangle[rectangles.Length];
       

            // For each of the specified hints, we try to pack and see if we can find a better solution.
            for (int i = 0; i < hints.Length && (!hasSolution || currentBestArea > acceptableBoundsArea); i++)
            {
                // We copy the rectangles onto the tmpBest array, then sort them by what the packing hint says.
                currentBest.CopyTo(tmpBest);
                
                PackingHintExtensions.SortByPackingHint(tmpBest, hints[i]);

                // We try to find the best bin for the rectangles in tmpBest. We give the function as
                // initial bin size the size of the best bin we got so far. The function never tries
                // bigger bin sizes, so if with a specified packingHint it can't pack smaller than
                // with the last solution, it simply stops.
                if (TryFindBestBin(emptySpaces, ref tmpBest, ref tmpArray, binWidth, binHeight, stepSize, acceptableBoundsArea,
                    out PackingRectangle boundsTmp))
                {
                    // We have a better solution!
                    // We update the variables tracking the current best solution
                    bounds = boundsTmp;
                    currentBestArea = boundsTmp.Area;
                    binWidth = bounds.Width;
                    binHeight = bounds.Height;

                    // We swap tmpBest and currentBest
                    Span<PackingRectangle> swaptmp = tmpBest;

                    tmpBest = currentBest;
                    currentBest = swaptmp;
                    hasSolution = true;
                }
            }

            if (!hasSolution)
                throw new Exception("Failed to find a solution. (Do your rectangles have a size close to uint.MaxValue or is your stepSize too high?)");

            // The solution should be in the "rectangles" array passed as parameter.
            if (currentBest != rectangles)
                currentBest.CopyTo(rectangles);

            // We return the list so it can be used in subsequent pack operations.
            ReturnList(emptySpaces);
        }

        /// <summary>
        /// Tries to find a solution with the smallest bin size possible, packing
        /// the rectangles in the order in which the were provided.
        /// </summary>
        /// <param name="emptySpaces">The list of empty spaces for reusing.</param>
        /// <param name="rectangles">The rectangles to pack. Might get swapped with "tmpArray".</param>
        /// <param name="tmpArray">A temporary array the function needs. Might get swapped with "rectangles".</param>
        /// <param name="binWidth">The maximum bin width to try.</param>
        /// <param name="binHeight">The maximum bin height to try.</param>
        /// <param name="stepSize">The amount by which to increment/decrement size when trying to pack another bin.</param>
        /// <param name="acceptableArea">Stops searching once a bin with this area or less is found.</param>
        /// <param name="bounds">The bounds of the resulting bin (0, 0, width, height).</param>
        /// <returns>Whether a solution was found.</returns>
        private static bool TryFindBestBin(List<PackingRectangle> emptySpaces, ref Span<PackingRectangle> rectangles,
            ref Span<PackingRectangle> tmpArray, uint binWidth, uint binHeight, uint stepSize, uint acceptableArea, out PackingRectangle bounds)
        {
            // We set boundsWidth and boundsHeight to these initial
            // values so they're not good enough for acceptableArea.
            uint boundsWidth = 0;
            uint boundsHeight = 0;
            bool isFirst = true;
            bounds = default;

            // We try packing the rectangles until we either fail, or find a solution with acceptable area.
            while ((isFirst || boundsWidth * boundsHeight > acceptableArea) &&
                    TryPackAsOrdered(emptySpaces, rectangles, tmpArray, binWidth, binHeight, out boundsWidth, out boundsHeight))
            {
                bounds.Width = boundsWidth;
                bounds.Height = boundsHeight;
                
                Span<PackingRectangle> swaptmp = rectangles;

                rectangles = tmpArray;
                tmpArray = swaptmp;

                // As we get close to the final result, we'll reduce the bin size by stepSize.
                binWidth = boundsWidth <= stepSize ? 1 : (boundsWidth - stepSize);
                binHeight = boundsHeight <= stepSize ? 1 : (boundsHeight - stepSize);
                isFirst = false;
            }

            // We return true if we've found any solution. Otherwise, false.
            return bounds.Width != 0 && bounds.Height != 0;
        }

        /// <summary>
        /// Tries to pack the rectangles in the given order into a bin of the specified size.
        /// </summary>
        /// <param name="emptySpaces">The list of empty spaces for reusing.</param>
        /// <param name="unpacked">The unpacked rectangles.</param>
        /// <param name="packed">Where the resulting rectangles will be written.</param>
        /// <param name="binWidth">The width of the bin.</param>
        /// <param name="binHeight">The height of the bin.</param>
        /// <param name="boundsWidth">The width of the resulting bin.</param>
        /// <param name="boundsHeight">The height of the resulting bin.</param>
        /// <returns>Whether the operation succeeded.</returns>
        /// <remarks>The unpacked and packed spans can be the same.</remarks>
        private static bool TryPackAsOrdered(List<PackingRectangle> emptySpaces, Span<PackingRectangle> unpacked,
            Span<PackingRectangle> packed, uint binWidth, uint binHeight, out uint boundsWidth, out uint boundsHeight)
        {
            // We clear the empty spaces list and add one space covering the entire bin.
            emptySpaces.Clear();
            emptySpaces.Add(new PackingRectangle(0, 0, binWidth, binHeight));

            // boundsWidth and boundsHeight both start at 0. 
            boundsWidth = 0;
            boundsHeight = 0;

            // We loop through all the rectangles.
            for (int r = 0; r < unpacked.Length; r++)
            {
                // We try to find a space for the rectangle. If we can't, then we return false.
                if (!TryFindBestSpace(unpacked[r], emptySpaces, out int spaceIndex))
                    return false;

                PackingRectangle oldSpace = emptySpaces[spaceIndex];
                packed[r] = unpacked[r];
                packed[r].X = oldSpace.X;
                packed[r].Y = oldSpace.Y;
                boundsWidth = Math.Max(boundsWidth, packed[r].Right);
                boundsHeight = Math.Max(boundsHeight, packed[r].Bottom);

                // We calculate the width and height of the rectangles from splitting the empty space
                uint freeWidth = oldSpace.Width - packed[r].Width;
                uint freeHeight = oldSpace.Height - packed[r].Height;

                if (freeWidth != 0 && freeHeight != 0)
                {
                    emptySpaces.RemoveAt(spaceIndex);
                    // Both freeWidth and freeHeight are different from 0. We need to split the
                    // empty space into two (plus the image). We split it in such a way that the
                    // bigger rectangle will be where there is the most space.
                    if (freeWidth > freeHeight)
                    {
                        emptySpaces.AddSorted(new PackingRectangle(packed[r].Right, oldSpace.Y, freeWidth, oldSpace.Height));
                        emptySpaces.AddSorted(new PackingRectangle(oldSpace.X, packed[r].Bottom, packed[r].Width, freeHeight));
                    }
                    else
                    {
                        emptySpaces.AddSorted(new PackingRectangle(oldSpace.X, packed[r].Bottom, oldSpace.Width, freeHeight));
                        emptySpaces.AddSorted(new PackingRectangle(packed[r].Right, oldSpace.Y, freeWidth, packed[r].Height));
                    }
                }
                else if (freeWidth == 0)
                {
                    // We only need to change the Y and height of the space.
                    oldSpace.Y += packed[r].Height;
                    oldSpace.Height = freeHeight;
                    emptySpaces[spaceIndex] = oldSpace;
                    EnsureSorted(emptySpaces, spaceIndex);
                    //emptySpaces.RemoveAt(spaceIndex);
                    //emptySpaces.Add(new PackingRectangle(oldSpace.X, oldSpace.Y + packed[r].Height, oldSpace.Width, freeHeight));
                }
                else if (freeHeight == 0)
                {
                    // We only need to change the X and width of the space.
                    oldSpace.X += packed[r].Width;
                    oldSpace.Width = freeWidth;
                    emptySpaces[spaceIndex] = oldSpace;
                    EnsureSorted(emptySpaces, spaceIndex);
                    //emptySpaces.RemoveAt(spaceIndex);
                    //emptySpaces.Add(new PackingRectangle(oldSpace.X + packed[r].Width, oldSpace.Y, freeWidth, oldSpace.Height));
                }
                else // The rectangle uses up the entire empty space.
                    emptySpaces.RemoveAt(spaceIndex);
            }

            return true;
        }

        /// <summary>
        /// Tries to find the best empty space that can fit the given rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle to find a space for.</param>
        /// <param name="emptySpaces">The list with the empty spaces.</param>
        /// <param name="index">The index of the space found.</param>
        /// <returns>Whether a suitable space was found.</returns>
        private static bool TryFindBestSpace(in PackingRectangle rectangle, List<PackingRectangle> emptySpaces, out int index)
        {
            for (int i = 0; i < emptySpaces.Count; i++)
                if (rectangle.Width <= emptySpaces[i].Width && rectangle.Height <= emptySpaces[i].Height)
                {
                    index = i;
                    return true;
                }

            index = -1;
            return false;
        }

        /// <summary>
        /// Gets a list of rectangles that can be used for empty spaces.
        /// </summary>
        /// <param name="preferredCapacity">If a list has to be created, this is used as initial capacity.</param>
        private static List<PackingRectangle> GetList(int preferredCapacity)
        {
            if (_oldListReference == null)
                return new List<PackingRectangle>(preferredCapacity);

            lock (_oldListReferenceLock)
            {
                if (_oldListReference.TryGetTarget(out List<PackingRectangle>? list))
                {
                    _oldListReference.SetTarget(null);
                    return list;
                }
                else
                {
                    return new List<PackingRectangle>(preferredCapacity);
                }
            }
        }

        /// <summary>
        /// Returns a list so it can be used in future pack operations. The list should
        /// no longer be used after returned.
        /// </summary>
        private static void ReturnList(List<PackingRectangle> list)
        {
            if (_oldListReference == null)
            {
                _oldListReference = new WeakReference<List<PackingRectangle>?>(list);
            }
            else
            {
                lock (_oldListReferenceLock)
                {
                    if (!_oldListReference.TryGetTarget(out List<PackingRectangle>? oldList) || oldList.Capacity < list.Capacity)
                    {
                        _oldListReference.SetTarget(list);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a rectangle to the list in sorted order.
        /// </summary>
        private static void AddSorted(this List<PackingRectangle> list, PackingRectangle rectangle)
        {
            rectangle.SortKey = Math.Max(rectangle.X, rectangle.Y);
            int max = list.Count - 1, min = 0;
            int middle, compared;

            // We perform a binary search for the space in which to add the rectangle
            while (min <= max)
            {
                middle = (max + min) / 2;
                compared = rectangle.SortKey.CompareTo(list[middle].SortKey);

                if (compared == 0)
                {
                    min = middle + 1;
                    break;
                }

                // If comparison is less than 0, rectangle should be inserted before list[middle].
                // If comparison is greater than 0, rectangle should be after list[middle].
                if (compared < 0)
                    max = middle - 1;
                else
                    min = middle + 1;
            }

            list.Insert(min, rectangle);
        }

        /// <summary>
        /// Updates an item's SortKey and ensures it is in the correct sorted position.
        /// If it's not, it is moved to the correct position.
        /// </summary>
        /// <remarks>If an item needs to be moved, it will only be moved forward. Never backwards.</remarks>
        private static void EnsureSorted(List<PackingRectangle> list, int index)
        {
            // We update the sort key. If it doesn't differ, we do nothing.
            uint newSortKey = Math.Max(list[index].X, list[index].Y);
            if (newSortKey == list[index].SortKey)
                return;

            int min = index;
            int max = list.Count - 1;
            int middle, compared;
            PackingRectangle rectangle = list[index];
            rectangle.SortKey = newSortKey;

            // We perform a binary search to look for where to put the rectangle.
            while (min <= max)
            {
                middle = (max + min) / 2;
                compared = newSortKey.CompareTo(list[middle].SortKey);

                if (compared == 0)
                {
                    min = middle - 1;
                    break;
                }

                // If comparison is less than 0, rectangle should be inserted before list[middle].
                // If comparison is greater than 0, rectangle should be after list[middle].
                if (compared < 0)
                    max = middle - 1;
                else
                    min = middle + 1;
            }
            min = Math.Min(min, list.Count - 1);

            // We have to place the rectangle in the index 'min'.
            for (int i = index; i < min; i++)
                list[i] = list[i + 1];

            list[min] = rectangle;
        }

        /// <summary>
        /// Calculates the sum of the Areas of all the given <see cref="PackingRectangle"/>-s.
        /// </summary>
        public static uint CalculateTotalArea(ReadOnlySpan<PackingRectangle> rectangles)
        {
            uint totalArea = 0;
            for (int i = 0; i < rectangles.Length; i++)
                totalArea += rectangles[i].Area;
            return totalArea;
        }

        /// <summary>
        /// Calculates the smallest possible rectangle that contains all the given rectangles.
        /// </summary>
        public static PackingRectangle FindBounds(ReadOnlySpan<PackingRectangle> rectangles)
        {
            PackingRectangle bounds = rectangles[0];
            for (int i = 1; i < rectangles.Length; i++)
            {
                bounds.X = Math.Min(bounds.X, rectangles[i].X);
                bounds.Y = Math.Min(bounds.Y, rectangles[i].Y);
                bounds.Right = Math.Max(bounds.Right, rectangles[i].Right);
                bounds.Bottom = Math.Max(bounds.Bottom, rectangles[i].Bottom);
            }

            return bounds;
        }

        /// <summary>
        /// Returns true if any two different rectangles in the given span intersect.
        /// </summary>
        public static bool AnyIntersects(ReadOnlySpan<PackingRectangle> rectangles)
        {
            for (int i = 0; i < rectangles.Length; i++)
                for (int c = i + 1; c < rectangles.Length; c++)
                    if (rectangles[c].Intersects(rectangles[i]))
                        return true;
            return false;
        }
    }
}
