using System;

namespace Machine.Specifications.Specs
{
  [Subject(typeof(ShouldExtensionMethods))]
  public class when_asserting_object_is_like_expected
  {
    class Dummy
    {
      public string Prop1 { get; set; }
      public int Prop2 { get; set; }
    }

    static Exception Exception;
    static Dummy Obj;
    
    Establish context = () => { Obj = new Dummy{Prop1 = "test", Prop2 = 2}; };

    public class with_correct_nodes_and_values
    {
      Because of = () => { Exception = Catch.Exception(() => Obj.ShouldBeLike(new {Prop1 = "test", Prop2 = 2})); };

      It should_not_throw = () => Exception.ShouldBeNull();
    }

    public class with_incorrect_node_value
    {
      Because of = () => { Exception = Catch.Exception(() => Obj.ShouldBeLike(new {Prop1 = "test2", Prop2 = 2})); };
      
      It should_throw = () => Exception.ShouldBeOfType<SpecificationException>();
      It should_contain_message = () => Exception.Message.ShouldEqual(
@"Property ""Prop1"":
  Expected string length 5 but was 4. Strings differ at index 4.
  Expected: ""test2""
  But was:  ""test""
  ---------------^");
    }
    
    public class with_missing_node
    {
      Because of = () => { Exception = Catch.Exception(() => Obj.ShouldBeLike(new {Prop1 = "test", Prop2 = 2, Prop3 = "other"})); };
      
      It should_throw = () => Exception.ShouldBeOfType<SpecificationException>();
      It should_contain_message = () => Exception.Message.ShouldEqual(
@"Property ""Prop3"":
  Expected: ""other""
  But was:  Not Defined");
    }

    public class with_multiple_errors
    {
      Because of = () => { Exception = Catch.Exception(() => Obj.ShouldBeLike(new {Prop1 = "test2", Prop2 = 3, Prop3 = "other"})); };
      
      It should_throw = () => Exception.ShouldBeOfType<SpecificationException>();
      It should_contain_message = () => Exception.Message.ShouldEqual(
@"Property ""Prop1"":
  Expected string length 5 but was 4. Strings differ at index 4.
  Expected: ""test2""
  But was:  ""test""
  ---------------^

Property ""Prop2"":
  Expected: [3]
  But was:  [2]

Property ""Prop3"":
  Expected: ""other""
  But was:  Not Defined");
    }
  }
}