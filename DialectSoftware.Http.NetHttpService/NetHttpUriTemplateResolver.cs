﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Http.Services
{
    public interface INetHttpUriTemplateResolver
    {
        String GetTemplate(Uri uri);
    }
}
