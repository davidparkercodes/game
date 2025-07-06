# Test Coverage Guide

This project includes a streamlined test coverage solution that provides clean, per-module coverage reporting.

## Quick Start

```bash
# Run coverage analysis for all modules
./test-coverage.sh

# Or using make
make coverage
```

**Output Example:**
```
🧪 Test Coverage Analysis
=========================

Domain: No tests found
Application: 75.2%
Di: No tests found  
Infrastructure: No tests found
Presentation: 45.7%

Total: 60.4%

✓ Coverage analysis complete
```

## Usage Options

### Basic Commands

```bash
# Full analysis
./test-coverage.sh

# Specific module only
./test-coverage.sh --module Application

# With threshold validation
./test-coverage.sh --threshold 70

# Detailed reports
./test-coverage.sh --detailed

# Help
./test-coverage.sh --help
```

### Makefile Targets

```bash
make coverage              # Full coverage analysis
make coverage-quick        # Quick analysis  
make coverage-ci           # CI with 50% threshold
make coverage-detailed     # With detailed reports
make coverage-application  # Application module only
make coverage-domain       # Domain module only
make coverage-di           # Di module only
make coverage-infrastructure # Infrastructure module only
make coverage-presentation # Presentation module only
make help                  # Show all targets
```

## Understanding Results

### Coverage Percentages
- **High (80%+)**: Excellent test coverage
- **Good (60-79%)**: Adequate coverage  
- **Low (40-59%)**: Needs more tests
- **Poor (<40%)**: Requires significant testing effort

### Module-Specific Expectations
- **Domain**: Should have highest coverage (business logic)
- **Application**: Good coverage expected (core functionality)
- **Infrastructure**: Lower coverage acceptable (external dependencies)
- **Presentation**: Lower coverage typical (UI components)
- **Di**: High coverage expected (dependency injection)

## Exit Codes

- `0`: Success
- `1`: Failure (build error, threshold not met, etc.)

## Threshold Validation

Use thresholds to enforce minimum coverage standards:

```bash
# Fail if total coverage below 70%
./test-coverage.sh --threshold 70

# CI-friendly threshold
make coverage-ci  # Uses 50% threshold
```

## Detailed Reports

Generate detailed coverage reports:

```bash
./test-coverage.sh --detailed
# Creates coverage files in ./coverage/ directory
```

## Configuration

Coverage settings are configured in:
- `Directory.Build.props` - Global MSBuild settings
- `coverlet.runsettings` - Detailed coverage configuration

### Excluded Files
- Test files (`*Tests*`, `*Test*`)
- Generated files (`*.Godot*`)
- Tool directories (`**/tools/**`)
- Build artifacts (`**/bin/**`, `**/obj/**`)

## Troubleshooting

### "No tests found"
- Module doesn't have test files in `tests/` directory
- This is normal for modules without tests yet

### "Build failed"
- Run `dotnet build` to see detailed error messages
- Ensure project compiles before running coverage

### "bc not found"
- Script will attempt to install `bc` on macOS
- Manual install: `brew install bc`

## Adding Tests

To improve coverage for a module:

1. Create test files in `tests/[ModuleName]/`
2. Write unit tests for the module's classes
3. Run coverage again to see improvements

Example test structure:
```
tests/
├── Application/
│   └── SomeServiceTests.cs
├── Domain/  
│   └── SomeEntityTests.cs
└── Infrastructure/
    └── SomeRepositoryTests.cs
```

## Integration with CI/CD

The coverage script is designed for CI/CD integration:

```yaml
# Example GitHub Actions step
- name: Run Coverage
  run: |
    chmod +x test-coverage.sh
    ./test-coverage.sh --threshold 60
```

## Files Created

- `test-coverage.sh` - Main coverage script
- `Makefile` - Easy execution targets
- `Directory.Build.props` - MSBuild coverage config
- `coverlet.runsettings` - Detailed coverage settings
- `coverage/` - Output directory (gitignored)
