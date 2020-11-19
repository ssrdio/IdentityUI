ALTER TABLE "Audit" ADD "GroupIdentifier" text NULL;

ALTER TABLE "Audit" ADD "Metadata" text NULL;

ALTER TABLE "Audit" ADD "SubjectMetadata" text NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20201029131921_AuditMetadataGroupIdentifier', '3.1.5');

