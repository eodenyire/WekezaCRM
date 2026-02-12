using FluentAssertions;
using WekezaCRM.Domain.Entities;
using Xunit;

namespace WekezaCRM.UnitTests.Entities;

public class CaseNoteEntityTests
{
    [Fact]
    public void CaseNote_Should_Initialize_With_Default_Values()
    {
        var note = new CaseNote();
        note.Note.Should().BeEmpty();
    }

    [Fact]
    public void CaseNote_Should_Set_Properties_Correctly()
    {
        var note = new CaseNote { Note = "Test Note", CaseId = Guid.NewGuid() };
        note.Note.Should().Be("Test Note");
    }

    [Fact]
    public void CaseNote_Should_Link_To_Case()
    {
        var caseId = Guid.NewGuid();
        var note = new CaseNote { CaseId = caseId };
        note.CaseId.Should().Be(caseId);
    }

    [Fact]
    public void CaseNote_Can_Be_Very_Long()
    {
        var longNote = new string('A', 5000);
        var note = new CaseNote { Note = longNote };
        note.Note.Should().HaveLength(5000);
    }

    [Fact]
    public void CaseNote_Can_Contain_Special_Characters()
    {
        var note = new CaseNote { Note = "Note with special chars: !@#$%^&*()" };
        note.Note.Should().Contain("!@#");
    }

    [Fact]
    public void CaseNote_Can_Contain_Line_Breaks()
    {
        var note = new CaseNote { Note = "Line 1\nLine 2\nLine 3" };
        note.Note.Should().Contain("\n");
    }

    [Fact]
    public void CaseNote_CreatedBy_Can_Be_Tracked()
    {
        var note = new CaseNote { CreatedBy = "Agent Smith" };
        note.CreatedBy.Should().Be("Agent Smith");
    }

    [Fact]
    public void CaseNote_CreatedAt_Can_Be_Set()
    {
        var now = DateTime.UtcNow;
        var note = new CaseNote { CreatedAt = now };
        note.CreatedAt.Should().Be(now);
    }

    [Fact]
    public void CaseNote_Can_Have_Null_UpdatedBy()
    {
        var note = new CaseNote { UpdatedBy = null };
        note.UpdatedBy.Should().BeNull();
    }

    [Fact]
    public void CaseNote_Can_Be_Updated()
    {
        var note = new CaseNote { Note = "Original", UpdatedAt = DateTime.UtcNow };
        note.UpdatedAt.Should().NotBeNull();
    }
}
