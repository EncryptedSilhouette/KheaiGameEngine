using System;

namespace KheaiGameEngine.Extensions
{
    public static class KCollectionExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var value in collection) action.Invoke(value);
            return collection;
        }

        public static T? Find<T>(this IEnumerable<T> collection, Predicate<T> match) 
        {
            foreach (var value in collection)
            {
                if (value is null) continue;
                if (match(value)) return value;
            }
            return default;
        }

        public static IEnumerable<T> FindAll<T>(this IEnumerable<T> collection, Predicate<T> match)
        {
            foreach (var value in collection)
            {
                if (value is null) continue;
                if (match(value)) yield return value;
            }
        }

        public static bool Contains<T>(this IEnumerable<T> collection, T match)
        {
            foreach (var value in collection)
            {
                if (value is null) continue;
                if (value.Equals(match)) return true;
            }
            return false;
        }

        ///<summary>Insert's an element into a sorted collection using a binary search.</summary>
        ///<param name = "item">The item to be inserted.</param>
        ///<param name = "comparison">The method of comparing 2 items.</param>
        //For the love of god don't change this. The slightest change fucks it all.
        public static void BinaryInsert<T>(this IList<T> collection, T item, Comparison<T> comparison)
        {
            if (item is null) return;

            //If the list is empty, add the item.
            if (collection.Count < 1)
            {
                collection.Add(item);
                return;
            }

            //If the compares the item with the last element.
            //If the item's value is greater than the last element's then add item to the end.
            //This is needed as insert won't work for this situation.
            if (comparison.Invoke(item, collection[collection.Count - 1]) >= 1)
            {
                collection.Add(item);
                return;
            }

            //For loop, define start index (iS), end index (iE), and current index (i).
            for (int iS = 0, iE = collection.Count, i = iE / 2; ;)
            {
                //Compares the item with the previous and current element.
                //Checks if it will fit between 2 elements.
                //If the index will be out of range, evaluate expression to true.
                //This will essentially remove the check.
                if ((i > 0 ? comparison.Invoke(item, collection[i - 1]) >= 0 : true) &&
                    (i < collection.Count ? comparison.Invoke(item, collection[i]) <= 0 : true))
                {
                    collection.Insert(i, item);
                    return;
                }

                //Range setting. (Adjusts iS & iE and sets i to the midpoint.)
                if (comparison.Invoke(item, collection[i]) < 0)
                {
                    iE = i; //Set the end index to current index.
                    //Decrement the index by half the range rounded up.
                    i -= (int) Math.Ceiling((iE - iS) / 2f);
                }
                else
                {
                    iS = i; //Set the start index to current index.
                    //Increment the index by half the range rounded up.
                    i += (int) Math.Ceiling((iE - iS) / 2f);
                }
            }
        }
    }
}
