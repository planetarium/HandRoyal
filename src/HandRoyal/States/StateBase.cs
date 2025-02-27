using System.ComponentModel.DataAnnotations;

namespace HandRoyal.States;

public abstract record class StateBase<T>
    where T : StateBase<T>
{
    public T Validate() => Validate(validateAllProperties: false);

    public T Validate(bool validateAllProperties)
    {
        var context = new ValidationContext(this);
        Validator.ValidateObject(this, context, validateAllProperties);
        return (T)this;
    }
}
