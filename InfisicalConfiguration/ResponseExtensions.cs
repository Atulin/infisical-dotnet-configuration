namespace InfisicalConfiguration;

public static class ResponseExtensions
{
	extension(HttpContent content)
	{
		public string ReadAsString()
		{
			var stream = content.ReadAsStream();
			using var reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}
	}
}