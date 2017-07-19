/*
 * The MIT License (MIT)
 * Copyright (c) 2016 Johan Karlsson
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights 
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is furnished 
 * to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 *
 */

using System;
using System.Linq;
using System.Collections.Generic;

namespace TinyPubSubLib
{
	public class Subscription
	{
		public Action Action { get; set; }
        public Action<string> ActionWithArgument { get; set; }
		public string Tag { get; set; }
		public object Owner { get; set; }

		public Subscription (Action action)
		{
			Action = action;
			Tag = Guid.NewGuid().ToString();
		}

        public Subscription (Action<string> action)
        {
            ActionWithArgument = action;
            Tag = Guid.NewGuid().ToString();
        }

		public Subscription (object owner, Action action)
		{
			Action = action;
			Tag = Guid.NewGuid().ToString();
			Owner = owner;
		}

        public Subscription (object owner, Action<string> action)
        {
            ActionWithArgument = action;
            Tag = Guid.NewGuid().ToString();
            Owner = owner;
        }
	}
}
