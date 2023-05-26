using Newtonsoft.Json;

namespace project_emih
{
    internal class Authenticator<T>
    {
        List<T> Authenticated = new List<T>();
        object _lock = new object();

        string _fileName = string.Empty;

        public Authenticator(string fileName)
        {
            _fileName = fileName;
            if (File.Exists(_fileName))
                Authenticated = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(_fileName));
            else
                File.WriteAllText(
                    _fileName,
                    JsonConvert.SerializeObject(Authenticated, Formatting.Indented)
                    );
            if (Authenticated == null)
                Authenticated = new List<T>();
        }

        public bool HasAuth(T param)
        {
            lock (_lock)
                return Authenticated.Contains(param);
        }
        public void Auth(T param)
        {
            lock (_lock)
                if (!HasAuth(param))
                    AddAuth(param);
        }

        void AddAuth(T param)
        {
            Authenticated.Add(param);
            File.WriteAllText(
                _fileName,
                JsonConvert.SerializeObject(Authenticated, Formatting.Indented)
                );
        }
    }
}
