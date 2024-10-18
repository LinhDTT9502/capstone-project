namespace _2Sport_BE.Helpers
{
    public interface IMethodHelper
    {
        void UpdateNonNullFields<T>(T source, T target);
    }
    public class MethodHelper : IMethodHelper
    {
        public void UpdateNonNullFields<T>(T source, T target)
        {
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var sourceValue = property.GetValue(source);
                if (sourceValue != null)
                {
                    property.SetValue(target, sourceValue);
                }
            }
        }
    }
}
