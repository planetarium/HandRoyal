using System.ComponentModel.DataAnnotations;

namespace HandRoyal.States;

public abstract record class StateBase<T>
    where T : StateBase<T>
{
    public T Validate() => Validate(recursive: false);

    public T Validate(bool recursive)
    {
        var context = new ValidationContext(this);
        Validator.ValidateObject(this, context, validateAllProperties: recursive);
        return (T)this;
    }
}
