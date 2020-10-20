ALTER TABLE "Audit" ADD "Metadata" text NULL;

ALTER TABLE "Audit" ADD "SubjectMetadata" text NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20201020220433_AddAuditMetadata', '3.1.5');

