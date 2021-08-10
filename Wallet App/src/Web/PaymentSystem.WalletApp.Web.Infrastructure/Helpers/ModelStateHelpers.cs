namespace PaymentSystem.WalletApp.Web.Infrastructure.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc.ModelBinding;

    using Newtonsoft.Json;

    using PaymentSystem.WalletApp.Web.Infrastructure.ModelStateSerialization;

    public static class ModelStateHelpers
    {
        public static string SerializeModelState(ModelStateDictionary modelState)
        {
            var errorList = modelState
                .Select(kvp => new ModelStateTransferValue
                {
                    Key = kvp.Key,
                    AttemptedValue = kvp.Value.AttemptedValue,
                    RawValue = kvp.Value.RawValue,
                    ErrorMessages = kvp.Value.Errors.Select(err => err.ErrorMessage).ToList(),
                });

            return JsonConvert.SerializeObject(errorList);
        }

        public static ModelStateDictionary DeserializeModelState(string serializedErrorList)
        {
            var errorList = JsonConvert.DeserializeObject<List<ModelStateTransferValue>>(serializedErrorList);
            var modelState = new ModelStateDictionary();

            foreach (var item in errorList)
            {
                modelState.SetModelValue(item.Key, item.RawValue, item.AttemptedValue);
                foreach (var error in item.ErrorMessages)
                {
                    modelState.AddModelError(item.Key, error);
                }
            }

            return modelState;
        }
    }
}
