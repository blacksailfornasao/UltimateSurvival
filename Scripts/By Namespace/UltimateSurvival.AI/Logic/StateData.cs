using System.Collections.Generic;

namespace UltimateSurvival.AI
{
    public class StateData
    {
        public Dictionary<string, object> m_Dictionary { get; private set; }


        public StateData() 
		{
			m_Dictionary = new Dictionary<string, object>(); 
		}

        public StateData(Dictionary<string, object> conditions) 
		{
			m_Dictionary = conditions; 
		}

        public void Add(string key, object value) 
		{
			m_Dictionary.Add(key, value);
		}

        public void Clear() 
		{
			m_Dictionary.Clear(); 
		}

        public static void OverrideValue(KeyValuePair<string, object> value, StateData data) 
		{
			data.m_Dictionary[value.Key] = value.Value;
		}

        public static void OverrideValue(string key, object value, StateData data)
        {
            KeyValuePair<string, object> pair = new KeyValuePair<string, object>(key, value);

            OverrideValue(pair, data);
        }

        public static void OverrideValues(StateData overrider, StateData data)
        {
            foreach (KeyValuePair<string, object> pair in overrider.m_Dictionary)
                data.m_Dictionary[pair.Key] = pair.Value;
        }
    }
}