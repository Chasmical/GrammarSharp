# GrammarSharp.Russian

[![Latest NuGet version](https://img.shields.io/nuget/v/GrammarSharp.Russian)](https://www.nuget.org/packages/GrammarSharp.Russian/)
[![MIT License](https://img.shields.io/github/license/Chasmical/GrammarSharp)](../LICENSE)

Provides a bunch of classes, structures and methods pertaining to Russian declension of nouns, adjectives and pronouns.



### Example

Simply grab a noun's declension class, and decline the word however you like!

```csharp
var noun = new RussianNoun("зверёк", new("мо 3*b"));

_ = noun.Decline(RussianCase.Dative, plural: false); // "зверьку"
_ = noun.Decline(RussianCase.Dative, plural: true); // "зверькам"
```

You can find declension classes for words in [Zaliznyak's online dictionary](https://gramdict.ru/search/*), or on [Wiktionary](https://ru.wiktionary.org/).

For quick reference, you can use this [short summary of Zaliznyak's classification](https://github.com/Chasmical/GrammarSharp/tree/main/GrammarSharp.Russian/README.Zaliznyak.md).



### Some more examples

```csharp
var noun = new RussianNoun("карько", new("мо <со 3*b(1)(2)>"));

_ = noun.Decline(RussianCase.Nominative, plural: true); // "карьки"
_ = noun.Decline(RussianCase.Genitive, plural: true); // "карьков"
```

```csharp
var noun = new RussianNoun("сестра", new("жо 1*d, ё"));
var adjective = new RussianAdjective("строгий", new("п 3a/c'"));

Console.WriteLine(
  adjective.Decline(RussianCase.Dative, plural: false, noun.Info.Properties) + " " +
  noun.Decline(RussianCase.Dative, plural: false)
);
// Output: "строгой сестре"

Console.WriteLine(
  adjective.Decline(RussianCase.Instrumental, plural: true, noun.Info.Properties) + " " +
  noun.Decline(RussianCase.Instrumental, plural: true)
);
// Output: "строгими сёстрами"
```


