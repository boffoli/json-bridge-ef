using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JsonBridgeEF.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JsonSchemaDefs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    JsonSchemaIdentifier = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JsonSchemaDefs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TargetDbContextDefs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Namespace = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetDbContextDefs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JsonFieldDefs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JsonSchemaDefId = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceFieldPath = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsKey = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JsonFieldDefs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JsonFieldDefs_JsonSchemaDefs_JsonSchemaDefId",
                        column: x => x.JsonSchemaDefId,
                        principalTable: "JsonSchemaDefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MappingProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    JsonSchemaDefId = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetDbContextDefId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MappingProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MappingProjects_JsonSchemaDefs_JsonSchemaDefId",
                        column: x => x.JsonSchemaDefId,
                        principalTable: "JsonSchemaDefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MappingProjects_TargetDbContextDefs_TargetDbContextDefId",
                        column: x => x.TargetDbContextDefId,
                        principalTable: "TargetDbContextDefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TargetPropertyDefs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TargetDbContextDefId = table.Column<int>(type: "INTEGER", nullable: false),
                    Namespace = table.Column<string>(type: "TEXT", nullable: false),
                    RootClass = table.Column<string>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetPropertyDefs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TargetPropertyDefs_TargetDbContextDefs_TargetDbContextDefId",
                        column: x => x.TargetDbContextDefId,
                        principalTable: "TargetDbContextDefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityKeyMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JsonKey = table.Column<string>(type: "TEXT", nullable: false),
                    TargetPropertyDefId = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityKeyMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityKeyMappings_TargetPropertyDefs_TargetPropertyDefId",
                        column: x => x.TargetPropertyDefId,
                        principalTable: "TargetPropertyDefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MappingRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MappingProjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    JsonFieldDefId = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetPropertyDefId = table.Column<int>(type: "INTEGER", nullable: false),
                    JsFormula = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MappingRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MappingRules_JsonFieldDefs_JsonFieldDefId",
                        column: x => x.JsonFieldDefId,
                        principalTable: "JsonFieldDefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MappingRules_MappingProjects_MappingProjectId",
                        column: x => x.MappingProjectId,
                        principalTable: "MappingProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MappingRules_TargetPropertyDefs_TargetPropertyDefId",
                        column: x => x.TargetPropertyDefId,
                        principalTable: "TargetPropertyDefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityKeyMappings_JsonKey_TargetPropertyDefId",
                table: "EntityKeyMappings",
                columns: new[] { "JsonKey", "TargetPropertyDefId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityKeyMappings_TargetPropertyDefId",
                table: "EntityKeyMappings",
                column: "TargetPropertyDefId");

            migrationBuilder.CreateIndex(
                name: "IX_JsonFieldDefs_JsonSchemaDefId_SourceFieldPath",
                table: "JsonFieldDefs",
                columns: new[] { "JsonSchemaDefId", "SourceFieldPath" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JsonSchemaDefs_JsonSchemaIdentifier",
                table: "JsonSchemaDefs",
                column: "JsonSchemaIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MappingProjects_JsonSchemaDefId",
                table: "MappingProjects",
                column: "JsonSchemaDefId");

            migrationBuilder.CreateIndex(
                name: "IX_MappingProjects_TargetDbContextDefId",
                table: "MappingProjects",
                column: "TargetDbContextDefId");

            migrationBuilder.CreateIndex(
                name: "IX_MappingRules_JsonFieldDefId",
                table: "MappingRules",
                column: "JsonFieldDefId");

            migrationBuilder.CreateIndex(
                name: "IX_MappingRules_MappingProjectId_JsonFieldDefId_TargetPropertyDefId",
                table: "MappingRules",
                columns: new[] { "MappingProjectId", "JsonFieldDefId", "TargetPropertyDefId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MappingRules_TargetPropertyDefId",
                table: "MappingRules",
                column: "TargetPropertyDefId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetDbContextDefs_Namespace",
                table: "TargetDbContextDefs",
                column: "Namespace",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TargetPropertyDefs_Namespace_RootClass_Path_Name",
                table: "TargetPropertyDefs",
                columns: new[] { "Namespace", "RootClass", "Path", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TargetPropertyDefs_TargetDbContextDefId",
                table: "TargetPropertyDefs",
                column: "TargetDbContextDefId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityKeyMappings");

            migrationBuilder.DropTable(
                name: "MappingRules");

            migrationBuilder.DropTable(
                name: "JsonFieldDefs");

            migrationBuilder.DropTable(
                name: "MappingProjects");

            migrationBuilder.DropTable(
                name: "TargetPropertyDefs");

            migrationBuilder.DropTable(
                name: "JsonSchemaDefs");

            migrationBuilder.DropTable(
                name: "TargetDbContextDefs");
        }
    }
}
