# Changelog

All notable changes to the SqlQueryBuilder project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.5.0] - 2024-11-01

### Added

- Support for .NET 9.0

### Changed

- Improved README.md documentation

### Removed

- Support for .NET 6.0 (end of life)

## [1.4.1] - 2024-06-15

### Changed

- License file inclusion in NuGet package

## [1.4.0] - 2024-01-15

### Added

- Support for .NET 8.0

## [1.3.1] - 2023-08-20

### Added

- Support for concatenation with assignment operators
- Structural equality implementation
- Method `SqlQueryBuilder.FromParameter` for creating instances with single parameter

### Changed

- Converted SqlQueryBuilder back to class from struct for better reference semantics

### Fixed

- Bug in nested composition scenarios

## [1.2.2] - 2023-05-10

### Added

- Support for "l" and "p" formats for string parameters to simplify inlining string literals

### Changed

- Converted SqlQueryBuilder from class to struct for better performance
- Reduced heap memory allocations for composite SQL queries

## [1.1.2] - 2023-03-15

### Added

- `ToString()` method support in SqlQueryBuilder

### Changed

- Required explicit conversion of SqlQueryBuilder from string to avoid ambiguity

## [1.1.1] - 2023-02-01

### Added

- Method `SqlQueryBuilder.FromParameter` for creating SqlQueryBuilder with single parameter

### Changed

- Renamed root namespace to avoid conflict with class name

### Removed

- Support for inline literal parameters (redundant API)

## [1.0.5] - 2022-12-10

### Added

- Support for inline literal parameters

## [1.0.4] - 2022-11-05

### Added

- Metadata support

## [1.0.3] - 2022-10-15

### Added

- Support for null parameters
- Parameter reuse for same values optimization

## [1.0.2] - 2022-09-20

### Added

- Configurable parameter name prefix

## [1.0.1] - 2022-09-01

### Added

- Initial release of SqlQueryBuilder