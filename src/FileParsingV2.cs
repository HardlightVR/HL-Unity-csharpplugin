﻿using System;

using Hardlight.Events;
using System.Linq;
using System.Collections.Generic;
using Hardlight.SDK.FileUtilities;
using UnityEngine;

namespace Hardlight.SDK.FileUtilities
{
	/// <summary>
	/// Represents a discriminated union of all possible area types
	/// Probably overkill for now..
	/// </summary>
	public class Area
	{
		public enum AreaType
		{
			Unknown,
			Region,
			AreaFlag
		}

		private AreaType tag;
		private List<int> regions;
		private AreaFlag flag;

		public AreaType Tag
		{
			get { return tag; }
		}

		/// <summary>
		/// Default construct an Area with value AreaFlag.None
		/// </summary>
		public Area()
		{
			tag = AreaType.AreaFlag;
			flag = AreaFlag.None;
		}

		/// <summary>
		/// Construct an Area from a given AreaFlag
		/// </summary>
		/// <param name="areaFlag"></param>
		public Area(AreaFlag areaFlag)
		{
			tag = AreaType.AreaFlag;
			flag = areaFlag;
		}

		/// <summary>
		/// Construct an Area from a given list of Regions
		/// </summary>
		/// <param name="regions"></param>
		public Area(List<int> regions)
		{
			tag = AreaType.Region;
			this.regions = new List<int>(regions);
		}

		/// <summary>
		/// Retrieve the value of the Area
		/// </summary>
		/// <param name="areaFlag"></param>
		/// <param name="regions"></param>
		public void Get(Action<AreaFlag> areaFlag, Action<List<int>> regions) 
		{
			switch (tag)
			{
				case AreaType.AreaFlag:
					areaFlag(flag);
					break;
				case AreaType.Region:
					regions(this.regions);
					break;
				default:
					break;
			}
		}
	}



	public class ParsingError : Exception
	{
		public ParsingError(string message) : base(message) { }
		public ParsingError(string message, Exception inner) : base(message, inner) { }
	} 

	/// <summary>
	/// First representation of the generic json data within a pattern. Leaves the arguments for a generator
	/// for later parsing. 
	/// </summary>
	public class RawEntry
	{
		public double Time;
		public string Sequence;
		public string AreaGenerator;
		public IDictionary<string, object> AreaGeneratorArgs; 
	}

	/// <summary>
	/// Second representation of json data within a pattern. Arguments for generator have been parsed, submitted to generator,
	/// and result in a list of Regions. 
	/// </summary>
	public class PatternEntry
	{
		public double Time;
		public string Sequence;
		public List<int> Area;

		public PatternEntry(RawEntry raw, List<int> area)
		{
			Time = raw.Time;
			Sequence = raw.Sequence;
			Area = area;
		}
	}

	
	public class InputModel
	{
		public List<PatternEntry> Entries;
	}


	public class InputModelParser
	{
		/// <summary>
		/// Represents a "generator" function that returns a list of areas given some user-defined parameters. 
		/// The generator is responsible for validating its own arguments and throwing ArgumentExceptions if necessary.
		/// </summary>
		/// <param name="arguments">Raw dictionary of parameters</param>
		/// <returns>List of areas</returns>
		public delegate List<int> GeneratorDelegate(IDictionary<string, object> arguments);

		static Dictionary<string, GeneratorDelegate> generators = new Dictionary<string, GeneratorDelegate>()
		{
			{"random", new GeneratorDelegate(random_generator) }
		};


		public class ParameterConstraint<T>
		{
			private string key;

			public ParameterConstraint(string key) {
				this.key = key;
			}

			/// <summary>
			/// Retrieve a list of T from the json, or return a default value
			/// Note: default integral type in the json is long, default floating point type is double
			/// </summary>
			/// <param name="arguments">arguments dictionary</param>
			/// <param name="defaultVal">default value</param>
			/// <returns></returns>
			public List<T> GetListOr(IDictionary<string, object> arguments, List<T> defaultVal)
			{
				

				if (arguments == null || !arguments.ContainsKey(key))
				{
					return defaultVal;
				}

				try
				{
					GetList<T>(arguments, key, out defaultVal);
					return defaultVal;
				} catch (ParsingError p)
				{
					throw new ArgumentException(string.Format("Parameter '{0}' parameter must be a {1}", key, typeof(T).FullName), p);
				}
			}
		
			/// <summary>
			/// Retrieve a T from the json, or return a default value
			/// Note: default integral type in the json is long, default floating point type is double
			/// </summary>
			/// <param name="arguments">arguments dictionary</param>
			/// <param name="defaultVal">default value</param>
			/// <returns></returns>
			public T GetOr(IDictionary<string, object> arguments, T defaultVal)
			{
				if (arguments == null || !arguments.ContainsKey(key))
				{
					return defaultVal;
				}

				try
				{
					Get(arguments, key, out defaultVal);
					return defaultVal;
				} catch (ParsingError p)
				{
					throw new ArgumentException(string.Format("Parameter '{0}' parameter must be a {1}", key, typeof(T).FullName), p);
				}
			}
		}

	

		public static List<int> random_generator(IDictionary<string, object> arguments)
		{
			System.Random gen = new System.Random();


			var areas = new ParameterConstraint<long>("area-set").GetListOr(arguments, Enumerable.Range(1, 16).ToList().Select(x => (long)x).ToList());

			int count = (int) new ParameterConstraint<long>("count").GetOr(arguments, 1);
			
			List<int> result = new List<int>();
			while (result.Count < count)
			{
				var pick = gen.Next((int)areas.Min(), (int)areas.Max() + 1);
				if (areas.Contains(pick)) {
					result.Add(pick);
				}
			}

			return result;
		}

	
		/// <summary>
		/// Invoke a generator by name with the given json arguments
		/// Throws ArgumentException if generatorName does not match any known generators
		/// </summary>
		/// <param name="generatorName">name of the generator</param>
		/// <param name="arguments">json arguments to the generator</param>
		/// <returns></returns>
		public static List<int> Invoke(string generatorName, IDictionary<string, object> arguments)
		{
			if (!generators.ContainsKey(generatorName))
			{
				throw new ArgumentException(string.Format("Could not find any generators of the name '{0}'", generatorName));
			}

			return generators[generatorName](arguments);
		}

	
		private static object GetRawHelper(IDictionary<string, object> arguments, string key)
		{
			object outVal = new object();
			if (!arguments.TryGetValue(key, out outVal))
			{
				throw new ParsingError(string.Format("Could not find required parameter '{0}'", key));
			}

			if (outVal == null)
			{
				throw new ParsingError(string.Format("Parameter '{0}' was null, malformed json?", key));
			}

			return outVal;
		}
		public static void GetList<T>(IDictionary<string, object> dict, string key, out List<T> value)
		{
			object outVal = GetRawHelper(dict, key);
			try
			{
				List<object> temp = (List<object>) outVal;
				value = temp.Select(x => (T) x).ToList();
			}
			catch (InvalidCastException)
			{
				throw new ParsingError(string.Format("Parameter '{0}' was not of type {1} as expected", key, typeof(T).FullName));
			}
		}
		public static void Get<T>(IDictionary<string, object> dict, string key, out T value) 
		{
			object outVal = GetRawHelper(dict, key);
			try
			{ 
				value = (T)outVal;
			}
			catch (InvalidCastException)
			{
				throw new ParsingError(string.Format("Parameter '{0}' was not of type {1} as expected", key, typeof(T).FullName));
			}
		}

		public static InputModel Parse(string json)
		{
			var dict = MiniJSON.Json.Deserialize(json) as IDictionary<string, object>;
			object pattern = null;
			if (!dict.TryGetValue("pattern", out pattern))
			{
				throw new ParsingError("Couldn't find required key 'pattern'");
			}

			return Parse(pattern as IList<object>);
		}

		/// <summary>
		/// Parse a RawEntry from the given raw json data
		/// </summary>
		/// <param name="rawEntry">raw json dictionary representing a RawEntry object</param>
		/// <returns></returns>
		public static RawEntry Parse(IDictionary<string, object> rawEntry)
		{
			RawEntry entry = new RawEntry();
			Get(rawEntry, "time", out entry.Time);
			Get(rawEntry, "sequence", out entry.Sequence);
			Get(rawEntry, "area", out entry.AreaGenerator);
			Get(rawEntry, "params", out entry.AreaGeneratorArgs);
			return entry;
		}

		private static InputModel Parse(IList<object> list)
		{
			InputModel model = new InputModel();
			model.Entries = new List<PatternEntry>();

			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] == null)
				{
					throw new ParsingError((string.Format("Entry [{0}] was malformed json", i)));
				}

				try
				{
					RawEntry raw = Parse(list[i] as IDictionary<string, object>);

					try
					{
						var areas = Invoke(raw.AreaGenerator, raw.AreaGeneratorArgs);

						model.Entries.Add(new PatternEntry(raw, areas));
					}
					catch (ArgumentException error)
					{
						throw new ParsingError(string.Format("In entry [{0}], while invoking generator: {1} ", i, error.Message), error);
					}

				} catch(ParsingError error)
				{
					throw new ParsingError(string.Format("In entry [{0}], while parsing: {1} ", i, error.Message), error);
				}
				


			}

			
			return model;
		}
	}
}
