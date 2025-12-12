# Keycloak Setup Guide

Since we do not import a realm automatically, the administrator must configure Keycloak manually.

## 1. Access the Admin Console
- **URL**: [http://localhost:8001](http://localhost:8001)
- **User**: `admin`
- **Password**: `admin`

## 2. Create the Realm
1.  Click the dropdown on the top-left (says **Master**).
2.  Click **Create Realm**.
3.  Name: `filoshop-realm`.
4.  Click **Create**.

## 3. Create the Client
1.  Go to **Clients** > **Create client**.
2.  **Client ID**: `filoshop-api`.
3.  **Capability config**:
    - [x] Client authentication (ON)
    - [x] Authorization (ON)
4.  Click **Save**.

## 4. Get Client Secret
1.  Go to **Clients** > `filoshop-api` > **Credentials** tab.
2.  Copy the **Client Secret**.
3.  Update `appsettings.json` or User Secrets with this value if needed for machine-to-machine communication.

## 5. Create Roles
1.  Go to **Realm roles**.
2.  Create roles:
    - `Member`
    - `Admin`

## 6. Create a Test User
1.  Go to **Users** > **Add user**.
2.  **Username**: `testuser`.
3.  **Email**: `test@example.com`.
4.  Click **Create**.
5.  Go to **Credentials** tab > **Set password**.
6.  Set password to `Password123!` (undo "Temporary").
