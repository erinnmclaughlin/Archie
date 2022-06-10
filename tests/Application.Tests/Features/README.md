# `CreateCustomerFeature` Unit Tests

<!--
Given that all external dependencies are behaving correctly, show that:

1. The `Create` method will throw an `ValidationException` when a `CreateCustomerRequest` fails validation.
1. The `Create` method will correctly convert from the given `CreateCustomerRequest` to a `Customer` object.
1. The `Create` method will add the correct audit event to the `Customer` object.
1. The `Create` method will commit the new `Customer` to persistence.
1. The `Create` method will return the correct `CreateCustomerResponse` object.
-->

## Test Case #1
***Show that "If the request is not valid, then `Create` throws a `ValidationException`".***

#### _Given:_
If the `CreateCustomerRequest` is not valid, then `IValidator<CreateCustomerRequest>` returns `ValidationRequest` where `IsValid == false`."

<hr />

#### _Proof:_

Let `A` = "The `Create` method throws a `ValidationException`."<br/>
Let `B` = "The `CreateCustomerRequest` is valid."<br/>
Let `C` = "`IValidator<CreateCustomerRequest>` returns `ValidationRequest` where `IsValid == false`."

##### `!B => C` (Given)
If the `CreateCustomerRequest` is not valid, then `IValidator<CreateCustomerRequest>` returns `ValidationRequest` where `IsValid == false`."
##### `C => A` (Proven via unit test)
If `IValidator<CreateCustomerRequest>` returns `ValidationRequest` where `IsValid == false`, then `Create` throws a `ValidationException`.
##### `!B => A` (Proven via transitive property)
If the `CreateCustomerRequest` is not valid, then `Create` throws a `ValidationException`.

<hr />

