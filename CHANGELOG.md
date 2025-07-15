## 1.5.0
- Support for .NET 9.0
- README.md is improved

## 1.4.1
- License file is included in nuget package

## 1.4.0
- Support .NET 8.0

## 1.3.1
- Support concatenation with assignment
- Support structural equality
- Convert SqlQueryBuilder back to class from struct
- Fix: bug in case of nested composition

## 1.2.2
- Support formats "l" and "p" for string parameters to simplify inlining string literals
- Convert SqlQueryBuilder from class to struct
- Reduce heap memory allocations for composite SQL queries

## 1.1.2
- Require explicit converting of SqlQueryBuilder from string to avoid ambiguity
- Support ToString in SqlQueryBuilder

## 1.1.1
- Rename root namespace to avoid conflict with class name
- Add method SqlQueryBuilder.FromParameter for creating SqlQueryBuilder with single parameter
- Remove support for inline literal parameters as redundant API

## 1.0.5
- Support inline literal parameters

## 1.0.4
- Support metadata

## 1.0.3
- Support null parameters
- Reuse parameters with same values

## 1.0.2
- Make parameter name prefix configurable

## 1.0.1
- Initial version