<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

This solution contains a Blazor WebAssembly frontend, an ASP.NET Core backend, and xUnit test projects for both. Use .NET 9 SDK for all development and testing.
Always check online documentation for the latest features and best practices.
Write and check unit tests for both frontend and backend to ensure code quality.
Use the az and gh CLI tools for Azure and GitHub operations, respectively.
Do not teach me but do the work yourself.
**ALWAYS deploy infrastructure and application using the GitHub Actions workflows in `.github/workflows`. NEVER use direct az CLI or local/agent-side deployment for infrastructure.**
Use the provided GitHub Actions workflows for CI/CD and all Azure deployments.
Use the `dotnet` CLI for building, testing, and running the projects.
Use the `dotnet watch` command for live reloading during development.
Use the `dotnet ef` CLI for database migrations and updates.
The commit messages should be clear and descriptive, following the conventional commits style.
after pushing changes to the main branch, the GitHub Actions workflows will automatically trigger deployments to Azure Static Web Apps and Azure Functions. 
Check for successful runs and also check the used resources in azure for correct configuration.
Lint all infrastructure code and use only bicep for infrastructure as code.
Lint and test the infrastructure before deploying.