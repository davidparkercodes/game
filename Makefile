# Test Coverage Makefile
# Simple targets for running coverage analysis

.PHONY: coverage coverage-quick coverage-ci coverage-detailed help

# Default target
coverage:
	@echo "Running test coverage analysis..."
	@./test-coverage.sh

# Quick coverage without detailed reports
coverage-quick:
	@./test-coverage.sh

# CI-friendly coverage with threshold
coverage-ci:
	@./test-coverage.sh --threshold 50

# Detailed coverage with HTML reports
coverage-detailed:
	@./test-coverage.sh --detailed

# Show help
help:
	@echo "Available targets:"
	@echo "  coverage         - Run full coverage analysis"
	@echo "  coverage-quick   - Run coverage without detailed reports"
	@echo "  coverage-ci      - Run coverage with CI threshold (50%)"
	@echo "  coverage-detailed- Run coverage with detailed HTML reports"
	@echo "  help            - Show this help"

# Module-specific targets
coverage-domain:
	@./test-coverage.sh --module Domain

coverage-application:
	@./test-coverage.sh --module Application

coverage-di:
	@./test-coverage.sh --module Di

coverage-infrastructure:
	@./test-coverage.sh --module Infrastructure

coverage-presentation:
	@./test-coverage.sh --module Presentation
