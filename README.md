# GitHub API Console App

A simple C# console application for learning, practicing and interacting with the GitHub API.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- GitHub account
- Git installed (optional, for cloning the repository)

## Setup Instructions

### Step 1: Clone the repository

```
git clone https://github.com/your-username/your-repository-name.git
```
cd your-repository-name

### Step 2: Install dependencies

```
dotnet restore
```

### Step 3: Create your GitHub Personal Access Token

1. Go to **GitHub Settings** → **Developer Settings** → **Personal access tokens** → **Tokens (classic)**.
2. Click **"Generate new token (classic)"**.
3. Set an expiration date and a descriptive name.
4. Grant the following permissions:
   - `repo`
   - `read:user`
5. Click **Generate Token** and **copy the token**.

> ⚠️ **Important:** You won't be able to see it again after leaving the page.


### Step 4: Configure your `appSettings.json`

1. Find the file `appSettingsExample.json` in the project directory.
2. Make a copy of it and rename the new file to `appSettings.json`.
3. Open `appSettings.json` and update it with your GitHub Personal Access Token:

```json
{
  "GitHubToken": "your_personal_access_token_here"
}
```

4. In your IDE:
   - Right-click on `appSettings.json`.
   - Set **Copy to Output Directory** → **Copy always**.

### Step 5: Build and run the application

Build the project:

```
dotnet build
```

