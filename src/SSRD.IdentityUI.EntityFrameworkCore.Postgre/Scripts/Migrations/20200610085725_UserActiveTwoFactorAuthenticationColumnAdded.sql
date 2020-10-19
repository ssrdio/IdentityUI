ALTER TABLE "Users" ADD "TwoFactor" integer NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200610085725_UserActiveTwoFactorAuthenticationColumnAdded', '3.1.5');

