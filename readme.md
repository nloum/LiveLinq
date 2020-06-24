<link rel="stylesheet" type="text/css" href="https://raw.githubusercontent.com/nloum/LiveLinq/develop/diffview.css">

# LiveLinq

![dotnetcore](https://img.shields.io/github/workflow/status/nloum/LiveLinq/dotnetcore) ![nuget](https://img.shields.io/nuget/v/LiveLinq) ![license](https://img.shields.io/github/license/nloum/LiveLinq) [![Join the chat at https://gitter.im/LiveLinq/community](https://badges.gitter.im/LiveLinq/community.svg)](https://gitter.im/LiveLinq/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

LiveLinq is the streaming LINQ library for .NET. The syntax looks mostly like standard LINQ, but it efficiently recalculates the resulting collections when the source collection changes.

For example:

<div id="diffoutput"><table class="diff" style="border-collapse: collapse; white-space: pre-wrap; border: 1px solid darkgray;">
<thead style="border-bottom-width: 1px; border-bottom-color: #BBC; border-bottom-style: solid; background-color: #EFEFEF; font-family: Verdana;"><tr>
<th></th>
<th class="texttitle" style="" align="left">Base Text</th>
<th></th>
<th class="texttitle" style="" align="left">New Text</th>
</tr></thead>
<tbody style="font-family: Courier, monospace;">
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">1</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">var source = new ObservableCollection&lt;int&gt;();</td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">1</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">var source = new ObservableCollection&lt;int&gt;();</td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">2</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">source.Add(3);</td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">2</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">source.Add(3);</td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">3</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">source.Add(4);</td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">3</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">source.Add(4);</td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">4</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top"></td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">4</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top"></td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">5</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">var evenNumbers = source</td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">5</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">var evenNumbers = source</td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top"></th>
<td class="empty" style="padding: .4em .4em 0px;" bgcolor="#DDD" valign="top"></td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">6</th>
<td class="insert" style="padding: .4em .4em 0px;" bgcolor="#9E9" valign="top">                        .ToLiveLinq()</td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">6</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">                        .Where(i =&gt; i % 2 == 0)</td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">7</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">                        .Where(i =&gt; i % 2 == 0)</td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">7</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">                        .ToReadOnlyObservableList();</td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">8</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">                        .ToReadOnlyObservableList();</td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">8</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top"></td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">9</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top"></td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">9</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">// This will print out 4</td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">10</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">// This will print out 4</td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">10</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">Console.WriteLine(string.Join(", ", evenNumbers.Select(i =&gt; i.ToString()));</td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">11</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">Console.WriteLine(string.Join(", ", evenNumbers.Select(i =&gt; i.ToString()));</td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">11</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top"></td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">12</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top"></td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">12</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">source.Add(5);</td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">13</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">source.Add(5);</td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">13</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">source.Add(6);</td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">14</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">source.Add(6);</td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">14</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top"></td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">15</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top"></td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">15</th>
<td class="delete" style="padding: .4em .4em 0px;" bgcolor="#E99" valign="top">// Now we need to recalculate evenNumbers, something we don't have to do with LiveLinq</td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top"></th>
<td class="empty" style="padding: .4em .4em 0px;" bgcolor="#DDD" valign="top"></td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">16</th>
<td class="delete" style="padding: .4em .4em 0px;" bgcolor="#E99" valign="top">evenNumbers = source.Where(i =&gt; i % 2 == 0)</td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top"></th>
<td class="empty" style="padding: .4em .4em 0px;" bgcolor="#DDD" valign="top"></td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">17</th>
<td class="delete" style="padding: .4em .4em 0px;" bgcolor="#E99" valign="top">                    .ToReadOnlyObservableList();</td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top"></th>
<td class="empty" style="padding: .4em .4em 0px;" bgcolor="#DDD" valign="top"></td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">18</th>
<td class="delete" style="padding: .4em .4em 0px;" bgcolor="#E99" valign="top"></td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top"></th>
<td class="empty" style="padding: .4em .4em 0px;" bgcolor="#DDD" valign="top"></td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">19</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">// This will print out 4, 6</td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">16</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">// This will print out 4, 6</td>
</tr>
<tr>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">20</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">Console.WriteLine(string.Join(", ", evenNumbers.Select(i =&gt; i.ToString()));</td>
<th style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EED" valign="top">17</th>
<td class="equal" style="padding: .4em .4em 0px;" valign="top">Console.WriteLine(string.Join(", ", evenNumbers.Select(i =&gt; i.ToString()));</td>
</tr>
<th class="author" colspan="4" style="font-family: verdana,arial,'Bitstream Vera Sans',helvetica,sans-serif; font-size: 11px; font-weight: normal; color: #886; padding: .3em .5em .1em 2em; border: 1px solid #bbc;" align="right" bgcolor="#EFEFEF" valign="top">diff view generated by <a href="http://github.com/cemerick/jsdifflib">jsdifflib</a>
</th>
</tbody>
</table></div>

In this example, the lambda `i => i % 2 == 0` gets called 6 times. First it's called with `i = 3`, once with `i = 4` when we're first defining the `evenNumbers` variable. Then it's called with `i` being `3`, `4`, `5`, and `6` again when we recalculate the `evenNumbers` variable.

This is how LiveLinq lets you work much more efficiently with complex LINQ statements on changing collections.

Most of the LINQ operators are supported: `SelectMany`, `Where`, `GroupBy`, etc. Also, different types of collections are supported: hashes, lists, sets, and dictionaries.
