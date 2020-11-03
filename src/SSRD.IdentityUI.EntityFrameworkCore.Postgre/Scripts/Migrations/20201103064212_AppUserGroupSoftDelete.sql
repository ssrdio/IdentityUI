ALTER TABLE "Users" ADD "_DeletedDate" timestamp with time zone NULL;

ALTER TABLE "Groups" ADD "_DeletedDate" timestamp with time zone NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20201103064212_AppUserGroupSoftDelete', '3.1.5');

