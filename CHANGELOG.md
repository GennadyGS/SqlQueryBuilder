# Changelog

All notable changes to the SqlQueryBuilder project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.5.0] - 2025-07-15

### Added

- Support for .NET 9.0

### Changed

- Improved README.md documentation

### Removed

- Support for .NET 6.0 (end of life)

## [1.4.1] - 2023-12-09

### Changed

- License file inclusion in NuGet package

## [1.4.0] - 2023-12-09

### Added

- Support for .NET 8.0

## [1.3.1] - 2023-04-29

### Added

- Support for concatenation with assignment operators
- Structural equality implementation
- Method `SqlQueryBuilder.FromParameter` for creating instances with single parameter

### Changed

- Converted SqlQueryBuilder back to class from struct for better reference semantics

### Fixed

- Bug in nested composition scenarios

## [1.2.2] - 2022-11-09

### Added

- Support for "l" and "p" formats for string parameters to simplify inlining string literals

### Changed

- Converted SqlQueryBuilder from class to struct for better performance
- Reduced heap memory allocations for composite SQL queries

## [1.2.1] - 2022-08-04

### Fixed

- Minor bug fixes and improvements

## [1.1.2] - 2022-08-02

### Added

- `ToString()` method support in SqlQueryBuilder

### Changed

- Required explicit conversion of SqlQueryBuilder from string to avoid ambiguity

## [1.1.1] - 2022-08-02

### Added

- Method `SqlQueryBuilder.FromParameter` for creating SqlQueryBuilder with single parameter

### Changed

- Renamed root namespace to avoid conflict with class name

### Removed

- Support for inline literal parameters (redundant API)

## [1.0.5] - 2022-08-01

### Added

- Support for inline literal parameters

## [1.0.4] - 2022-03-26

### Added

- Metadata support

## [1.0.3] - 2022-02-07

### Added

- Support for null parameters
- Parameter reuse for same values optimization

## [1.0.2] - 2022-01-22

### Added

- Configurable parameter name prefix

## [1.0.1] - 2022-01-16

### Added

- Initial release of SqlQueryBuilder
